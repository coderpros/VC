using Microsoft.Extensions.FileProviders;
using Microsoft.Net.Http.Headers;
using SixLabors.ImageSharp.Web.DependencyInjection;
using SixLabors.ImageSharp.Web.Providers;
using Syncfusion.Blazor;
using VC.Res.WebInterface.Middleware;
using SixLabors.ImageSharp.Web.Caching;
using Microsoft.AspNetCore.DataProtection;
using RazorComponentsPreview;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDataProtection().PersistKeysToFileSystem(new DirectoryInfo(builder.Configuration.GetValue<string>("FilePaths:DPKeys:Physical")));

builder.Services.AddLocalization();

// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.AddServerSideBlazor(options => { options.DisconnectedCircuitRetentionPeriod = TimeSpan.FromMinutes(35); })
    .AddCircuitOptions(option => { option.DetailedErrors = true; })
    .AddHubOptions(option => { option.MaximumReceiveMessageSize = 2 * 1024 * 1024; }); // sets max message size to 2mb (this is to fix alot of content in content editor causing disconnect issues)

builder.Services.AddSyncfusionBlazor();
builder.Services.AddRazorComponentsRuntimeCompilation();

builder.Services.AddImageSharp()
    .RemoveProvider<PhysicalFileSystemProvider>()
    .AddProvider<CustomPhysicalFileSystemProvider>()
    .Configure<CustomPhysicalFileSystemProviderOptions>(options =>
    {
        options.Paths = new Dictionary<string, string> { { builder.Configuration.GetValue<string>("FilePaths:ContentFiles:Virtual"), builder.Configuration.GetValue<string>("FilePaths:ContentFiles:Physical") } };
    })
    .Configure<PhysicalFileSystemCacheOptions>(options =>
    {
        options.CacheRootPath = null;
        options.CacheFolder = "is-cache";
        options.CacheFolderDepth = 5;
    })
    .SetCache<PhysicalFileSystemCache>()
    .SetCacheKey<UriRelativeLowerInvariantCacheKey>()
    .SetCacheHash<SHA256CacheHash>();

builder.Services.AddWebOptimizer(pipeline =>
{
    pipeline.AddCssBundle("/css/bundle.css", "css/stylesheet.css", "css/bootstrap5.css", "css/content-editor.css");
    pipeline.AddJavaScriptBundle("/js/bundle.js",
        "js/jquery-3.6.0.min.js",
        "js/libs/modernizr-custom.js",
        "js/libs/jquery.mousewheel.min.js",
        "js/libs/jquery.scrollbar.js",
        "js/libs/dragscroll.js",
        "js/custom/sessionManagement.js",
        "js/custom/main.js");
});

builder.Services.AddScoped<VC.Res.WebInterface.Services.NiceUIService>();
builder.Services.AddScoped<VC.Res.WebInterface.Services.SessionInfoService>();
builder.Services.AddControllersWithViews().AddRazorRuntimeCompilation();
//builder.Services.AddScoped<VC.Res.WebInterface.Models.SessionInfo>();

// fix for usestaticfiles not allowing options (as breaks Blazor key js file)
builder.Services.Configure<StaticFileOptions>(options => { options.OnPrepareResponse = (response) => { response.Context.Response.Headers.Append("Cache-Control", "public,max-age=7776000"); }; });




var app = builder.Build();
//app.UseRazorComponentsRuntimeCompilation();
app.UseRequestLocalization("en-GB");

// Register Syncfusion license (21.2.4)
Syncfusion.Licensing.SyncfusionLicenseProvider.RegisterLicense("MTk5ODQzOEAzMjMxMmUzMjJlMzRPaGgwTm1tUkhvRVd5TVI0THN2OVFoUDVEcFdJaEpZd2JsRkNkS2pRRFpZPQ==");

// Configure the HTTP request pipeline.
if (app.Environment.IsEnvironment("Production") || app.Environment.IsEnvironment("UAT"))
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}
else
{
    app.UseDeveloperExceptionPage();
}


app.UseHttpsRedirection();

app.UseImageSharp();
app.UseWebOptimizer();

//app.UseStaticFiles();
app.UseStaticFiles(new StaticFileOptions
{
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 7;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});

app.UseStaticFiles(new StaticFileOptions
{
    FileProvider = new PhysicalFileProvider(builder.Configuration.GetValue<string>("FilePaths:ContentFiles:Physical")),
    RequestPath = builder.Configuration.GetValue<string>("FilePaths:ContentFiles:Virtual"),
    OnPrepareResponse = ctx =>
    {
        const int durationInSeconds = 60 * 60 * 24 * 7;
        ctx.Context.Response.Headers[HeaderNames.CacheControl] =
            "public,max-age=" + durationInSeconds;
    }
});

app.UseStaticFiles();

app.UseRouting();

//app.UseSessionInfo();
app.MapBlazorHub();
app.MapFallbackToPage("/_Host");

app.UseWhen(context => context.Request.Path.StartsWithSegments("/api/website"), appBuilder =>
{
    _ = appBuilder.UseMiddleware<APIKeyMiddleware>();
});

app.UseEndpoints(endpoints => { endpoints.MapControllers(); });

app.Run();

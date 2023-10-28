using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Options;
using SixLabors.ImageSharp.Web.Resolvers;

namespace SixLabors.ImageSharp.Web.Providers
{
    /// <summary>
    /// Returns images stored in the local physical file system.
    /// </summary>
    public class CustomPhysicalFileSystemProvider : IImageProvider
    {
        /// <summary>
        /// The file provider abstraction.
        /// </summary>
        private readonly IFileProvider _fileProvider;

        private readonly Dictionary<string, IFileProvider> _customProviders;

        /// <summary>
        /// Contains various format helper methods based on the current configuration.
        /// </summary>
        private readonly FormatUtilities _formatUtilities;

        private readonly Dictionary<string, string> _customPaths;

        /// <summary>
        /// A match function used by the resolver to identify itself as the correct resolver to use.
        /// </summary>
        private Func<HttpContext, bool> _match;

        /// <summary>
        /// Initializes a new instance of the <see cref="PhysicalFileSystemProvider"/> class.
        /// </summary>
        /// <param name="environment">The environment used by this middleware.</param>
        /// <param name="formatUtilities">Contains various format helper methods based on the current configuration.</param>
        public CustomPhysicalFileSystemProvider(IWebHostEnvironment environment, FormatUtilities formatUtilities, IOptions<CustomPhysicalFileSystemProviderOptions> customPaths)
        {
            //Guard.NotNull(environment, nameof(environment));
            //Guard.NotNull(environment.WebRootFileProvider, nameof(environment.WebRootFileProvider));

            this._fileProvider = environment.WebRootFileProvider;
            this._formatUtilities = formatUtilities;

            this._customProviders = new Dictionary<string, IFileProvider>();
            this._customPaths = new Dictionary<string, string>();

            if (customPaths != null)
            {
                foreach(var kvp in customPaths.Value.Paths)
                {
                    this._customPaths.Add(kvp.Key, kvp.Value);
                    this._customProviders.Add(kvp.Key, new PhysicalFileProvider(kvp.Value));
                }
            }

            //this._fileProvider = string.IsNullOrWhiteSpace(mediaOptions.Value.ContentPath) ? environment.WebRootFileProvider : new PhysicalFileProvider(mediaOptions.Value.ContentPath);
            

            //this._requestPath = mediaOptions.Value.RequestPath;
        }

        /// <inheritdoc/>
        public ProcessingBehavior ProcessingBehavior { get; } = ProcessingBehavior.CommandOnly;

        /// <inheritdoc/>
        public Func<HttpContext, bool> Match { get; set; } = _ => true;

        ///// <inheritdoc/>
        //public Func<HttpContext, bool> Match
        //{
        //    //get { return string.IsNullOrWhiteSpace(_requestPath) ? _ => true : _match ?? IsMatch; }

        //    get { return IsMatch; }

        //    set => _match = value;
        //}

        /// <inheritdoc/>
        public bool IsValidRequest(HttpContext context) => this._formatUtilities.TryGetExtensionFromUri(context.Request.GetDisplayUrl(), out _);

        /// <inheritdoc/>
        public Task<IImageResolver> GetAsync(HttpContext context)
        {
            // from the content/path, we need to work out which file provider to use

            var providerToUse = this._fileProvider;
            var path = "";

            if (context.Request.Path.HasValue)
            {
                path = context.Request.Path.Value;

                foreach (var customProvider in this._customProviders)
                {
                    if (context.Request.Path.Value.StartsWith(customProvider.Key))
                    {
                        providerToUse = customProvider.Value;
                        path = context.Request.Path.Value.Substring(customProvider.Key.Length);
                    }
                }
            }

            // Remove assets request path if it's set.
            //string path = string.IsNullOrWhiteSpace(_requestPath) ? context.Request.Path.Value : context.Request.Path.Value.Substring(_requestPath.Value.Length);

            // Path has already been correctly parsed before here.
            //IFileInfo fileInfo = this._fileProvider.GetFileInfo(context.Request.Path.Value);

            IFileInfo fileInfo = providerToUse.GetFileInfo(path);

            // Check to see if the file exists.
            if (!fileInfo.Exists)
            {
                return Task.FromResult<IImageResolver>(null);
            }

            var metadata = new ImageMetadata(fileInfo.LastModified.UtcDateTime, fileInfo.Length);
            //return Task.FromResult<IImageResolver>(new PhysicalFileSystemResolver(fileInfo, metadata));
            return Task.FromResult<IImageResolver>(new FileProviderImageResolver(fileInfo));
        }

        //private bool IsMatch(HttpContext context)
        //{
        //    // from the content/path, we need to work out which file provider to use

        //    //if (!context.Request.Path.StartsWithNormalizedSegments(_requestPath, StringComparison.OrdinalIgnoreCase))
        //    //{
        //    //    return false;
        //    //}

        //    return true;
        //}
    }

    public class CustomPhysicalFileSystemProviderOptions
    {
        public Dictionary<string, string> Paths { get; set; }
    }
}

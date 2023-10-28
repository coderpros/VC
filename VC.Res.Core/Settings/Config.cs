using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using VC.Res.Core.Database;

namespace VC.Res.Core.Settings
{
    internal static class Config
    {
        private static readonly DbContextOptions<DBContext> contextOptions;


        static Config()
        {
            var env = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .AddJsonFile($"appsettings.{env}.json", optional: true)
                .Build();
            //configuration = builder.Build();

            contextOptions = new DbContextOptionsBuilder<DBContext>().UseSqlServer(configuration.GetConnectionString("Default")).Options;
        }

        public static DBContext DBConnection()
        {
            return new DBContext(contextOptions);
        }

        public static DBContext DBPooledConnection()
        {
            return new PooledDbContextFactory<DBContext>(contextOptions).CreateDbContext();
        }
    }
}

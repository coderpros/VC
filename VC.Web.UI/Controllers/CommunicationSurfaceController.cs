// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommunicationSurfaceController.cs" company="Villa Collective ">
//   Copyright 2023 VillaCollective.com. All rights reserved.
// </copyright>
// <summary>
//   Defines the CommunicationSurfaceController type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace VillaCollective.Web.UI.Controllers
{
    using Microsoft.AspNetCore.Mvc;
    using Umbraco.Cms.Core.Cache;
    using Umbraco.Cms.Core.Logging;
    using Umbraco.Cms.Core.Routing;
    using Umbraco.Cms.Core.Services;
    using Umbraco.Cms.Core.Web;
    using Umbraco.Cms.Infrastructure.Persistence;
    using Umbraco.Cms.Web.Website.Controllers;

    /// <inheritdoc />
    /// <summary>
    /// The communication surface controller.
    /// </summary>
    public class CommunicationSurfaceController : SurfaceController
    {
        #region Fields

        /// <summary>
        /// The configuration interface.
        /// </summary>
        private readonly IConfiguration configuration;

        /// <summary>
        ///  The email configuration model.
        /// </summary>
        private readonly Models.EmailConfiguration emailConfig;

        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CommunicationSurfaceController"/> class.
        /// </summary>
        /// <param name="umbracoContextAccessor">
        /// The umbraco context accessor.
        /// </param>
        /// <param name="databaseFactory">
        /// The database factory.
        /// </param>
        /// <param name="services">
        /// The services.
        /// </param>
        /// <param name="appCaches">
        /// The app caches.
        /// </param>
        /// <param name="profilingLogger">
        /// The profiling logger.
        /// </param>
        /// <param name="publishedUrlProvider">
        /// The published url provider.
        /// </param>
        /// <param name="configuration">
        /// A reference to the site's configuration.
        /// </param>
        /// <param name="emailConfig">
        /// The email configuration model.
        /// </param>
        public CommunicationSurfaceController(
            IUmbracoContextAccessor umbracoContextAccessor,
            IUmbracoDatabaseFactory databaseFactory,
            ServiceContext services,
            AppCaches appCaches,
            IProfilingLogger profilingLogger,
            IPublishedUrlProvider publishedUrlProvider,
            IConfiguration configuration,
            Models.EmailConfiguration emailConfig)
            : base(umbracoContextAccessor, databaseFactory, services, appCaches, profilingLogger, publishedUrlProvider)
        {
            this.configuration = configuration;
            this.emailConfig = emailConfig;
        }

        #endregion

        [HttpPost]
        public async Task Contact()
        {

        }
    }
}

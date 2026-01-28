//API versioning config
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;

namespace ApartmentManagementSystem.API.Configuration
{
    public static class ApiVersioningConfiguration
    {
        public static IServiceCollection AddApiVersioningConfiguration(this IServiceCollection services)
        {
            services.AddApiVersioning(options =>
            {
                // Default version when not specified
                options.DefaultApiVersion = new ApiVersion(1, 0);

                // Assume default version when client doesn't specify
                options.AssumeDefaultVersionWhenUnspecified = true;

                // Report available versions in response headers
                options.ReportApiVersions = true;

                // URL Segment versioning: /api/v1/controller
                options.ApiVersionReader = new UrlSegmentApiVersionReader();
            });

            // API Explorer for Swagger documentation
            services.AddVersionedApiExplorer(options =>
            {
                // Format: 'v'major[.minor]
                options.GroupNameFormat = "'v'VVV";

                // Substitute version in URL
                options.SubstituteApiVersionInUrl = true;
            });

            return services;
        }
    }
}

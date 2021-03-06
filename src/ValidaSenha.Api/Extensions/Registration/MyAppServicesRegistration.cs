using Microsoft.Extensions.Configuration;
using ValidaSenha.Api.Infraestructure;
using ValidaSenha.Api.Services;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class MyAppServicesRegistration
    {
        public static IServiceCollection AddToAppServicesRegistration(this IServiceCollection services,
                                                         IConfiguration configuration)
        {
            services.AddSingleton<MetricReporter>();
            services.AddScoped<ValidaSenhaService>();
            return services;
        }
    }
}
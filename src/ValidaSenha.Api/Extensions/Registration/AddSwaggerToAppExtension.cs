using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class CustomSwaggerAppExtension
    {

        private static readonly string _swaggerDocName = "ItiValidaSenhaOpenApiSpecification";
        private static readonly string _swaggerDocTitle = "API para Validar Complexidade de Senha";
        private static readonly string _swaggerDocVersion = "v1";
        private static readonly string _swaggerDocUrl = "/swagger/ItiValidaSenhaOpenApiSpecification/swagger.json";

        public static IServiceCollection AddToAppSwagger(this IServiceCollection services,
                                                         IConfiguration configuration)
        {
            services.AddSwaggerGen(setupAction => 
            {
                setupAction.SwaggerDoc(_swaggerDocName, new OpenApiInfo()
                {
                    Title = _swaggerDocTitle,
                    Version = _swaggerDocVersion,
                    Description = "Some description goes here",
                    Contact = new OpenApiContact()
                    {
                        Name = "Carlos Decloedt Jr",
                        Email = "carlos@decloedtjr.com.br",
                        Url = new Uri("http://www.decloedtjr.com.br")
                    }
                });

                // Gera documentação do swagger a partir da documentação xml das classes e actions
                var xmlCommentsFile = Path.Combine(
                    AppContext.BaseDirectory, 
                    $"{Assembly.GetExecutingAssembly().GetName().Name}.xml");
                setupAction.IncludeXmlComments(xmlCommentsFile);
            });
            return services;
        }

        public static IApplicationBuilder UseSwaggerOnApp(this IApplicationBuilder app)
        {
            app.UseSwagger();
            app.UseSwaggerUI(opts => 
            {
                opts.RoutePrefix = "";
                opts.SwaggerEndpoint(url: _swaggerDocUrl, name: _swaggerDocName);
            });
            return app;
        }
    }
}
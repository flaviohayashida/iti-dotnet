using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using ValidaSenha.Api.Utils;
using FluentValidation.AspNetCore;
using System.Reflection;
using Prometheus;
using ValidaSenha.Api.Infraestructure;

namespace ValidaSenha.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddCors(GetAddCorsConfig);

            services.AddToAppSwagger(this.Configuration); // Extension para facilitar e deixar o startup limpo
            services.AddToAppServicesRegistration(this.Configuration); // Extension para facilitar e deixar o startup limpo
            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

            services.AddControllers(opts =>
            {
                opts.ReturnHttpNotAcceptable = true;
            })
            .AddNewtonsoftJson(GetAddNewtonsoftJson)
            .ConfigureApiBehaviorOptions(GetApiBehaviorOptionsConfiguration);
        }


        public void Configure(IApplicationBuilder app,
                              IWebHostEnvironment env,
                              IHostApplicationLifetime lifetime
                              )
        {
            app.UseCors("CorsPolicy");
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseExceptionHandler(opts =>
                {
                    // TODO: Mudar a implantação abaixo e implantar o ProblemDetails, logging, etc
                    opts.Run(async (context) =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("Oh No!");
                    });
                });
            }
            app.UseHttpsRedirection();
            app.UseSwaggerOnApp(); // Extension para facilitar e deixar o startup limpo
            app.UseResponseCaching();
            app.UseRouting();
            app.UseAuthorization();
            app.UseMetricServer();
            app.UseMiddleware<ResponseMetricMiddleware>();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

        }



        private void GetAddCorsConfig(CorsOptions corsOptions)
        {
            corsOptions.AddPolicy("CorsPolicy", configurePolicy =>
            {
                configurePolicy.AllowAnyOrigin();
                configurePolicy.AllowAnyHeader();
                configurePolicy.AllowAnyMethod();
            });
        }

        private void GetAddNewtonsoftJson(MvcNewtonsoftJsonOptions setupAction)
        {
            setupAction.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
            setupAction.SerializerSettings.Formatting = Formatting.None;
            setupAction.SerializerSettings.NullValueHandling = NullValueHandling.Include;
            setupAction.SerializerSettings.MissingMemberHandling = MissingMemberHandling.Ignore;
        }

        private void GetApiBehaviorOptionsConfiguration(ApiBehaviorOptions options)
        {
            options.InvalidModelStateResponseFactory = context =>
            {
                var problemDetails = new ValidationProblemDetails(context.ModelState)
                {
                    Type = "http://itivalidacomplexidadesenha/modelvalidationproblem",
                    Title = "One or more model validation errors occurred.",
                    Status = StatusCodes.Status422UnprocessableEntity,
                    Detail = "See the errors property for details.",
                    Instance = context.HttpContext.Request.Path
                };
                problemDetails.Extensions.Add("traceId", context.HttpContext.TraceIdentifier);
                return new UnprocessableEntityObjectResult(problemDetails)
                {
                    ContentTypes = { "application/problem+json" }
                };
            };
        }
    }
}

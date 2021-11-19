using CourseLibrary.Api.Models;
using CourseLibrary.Api.Models.Core.Repositories;
using CourseLibrary.Api.Models.Persistence;
using CourseLibrary.Api.Services;
using Marvin.Cache.Headers;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CourseLibrary.Api
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // Supports Cache headers with ETag
           // services.AddHttpCacheHeaders((expirationModelOptions) =>
           // {
           //     expirationModelOptions.MaxAge = 60;
           //     expirationModelOptions.CacheLocation = Marvin.Cache.Headers.CacheLocation.Private;
           // },
           //(validationModelOptions) =>
           //{
           //    validationModelOptions.MustRevalidate = false;
           //});


            // Supports Caching
            services.AddResponseCaching();

            // Inject the services of AutoMapper
            services.AddAutoMapper(AppDomain.CurrentDomain.GetAssemblies());

            // Inject the Course Library Repository through the project
            services.AddScoped<ICourseLibraryRepository, CourseLibraryRepository>();

            // Inject the Mapping Service 
            services.AddTransient<IPropertyMappingService, PropertyMappingService>();

            // Inject the Property Existence Checker
            services.AddTransient<IPropertyExistenceChecker, PropertyExistenceChecker>();

            // Inject the App Data Context through the project
            services.AddDbContext<AppDataContext>(options => 
            options.UseSqlServer(Configuration.GetConnectionString("Default")));

            // Supporting Xml media type, returning 406 not acceptable, and convert the patch document succefully
            services.AddControllers(setup =>
            {
                setup.ReturnHttpNotAcceptable = true;
                setup.CacheProfiles.Add("60SecondsCacheProfile", new CacheProfile()
                {
                    Duration = 60
                });


            }).AddNewtonsoftJson(setupAction =>
            {
                setupAction.SerializerSettings.ContractResolver =
                    new CamelCasePropertyNamesContractResolver();
            })
               .ConfigureApiBehaviorOptions(setupAction =>
               {
                   setupAction.InvalidModelStateResponseFactory = context =>
                   {
                       // Create problem details object
                       var problemDetailsFactory = context.HttpContext.RequestServices
                           .GetRequiredService<ProblemDetailsFactory>();

                       var problemDetails = problemDetailsFactory.CreateValidationProblemDetails(
                           context.HttpContext,
                           context.ModelState);

                       // Set Additional Info
                       problemDetails.Detail = "See the Errors Field for Deatails";
                       problemDetails.Instance = context.HttpContext.Request.Path;

                       // Get ActioneExecutingContext Object
                       var actionExecutingContext = context as Microsoft.AspNetCore.Mvc.Filters.ActionExecutingContext;

                       // Check if there's validation errors
                       // and if the body not empty or any property not parsed
                       // return details and 422 status code
                       if ((context.ModelState.ErrorCount > 0) &&
                         (context is ControllerContext || actionExecutingContext?.ActionArguments.Count
                         == context.ActionDescriptor.Parameters.Count))
                       {
                           problemDetails.Type = "https://courselibrary.com/modelvalidationproblem";
                           problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                           problemDetails.Title = "one or more validation errors occured";

                           return new UnprocessableEntityObjectResult(problemDetails)
                           {
                               ContentTypes = { "application/problem+json" }
                           };
                       }

                       // If it's parsing or body empty problem
                       // Return details and 400 status code
                       problemDetails.Status = StatusCodes.Status400BadRequest;
                       problemDetails.Title = "one or more errors on input occured";

                       return new BadRequestObjectResult(problemDetails)
                       {
                           ContentTypes = { "application/problem+json" }
                       };

                   };
               });

            services.AddMvc();
            // Add a new media type to support
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters.
                OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add(MediaTypeHeaderValue.Parse("application/vnd.marvin.hateoas+json"));
                }
            });
            
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            else
            {
                app.UseExceptionHandler(appBuilder =>
                {
                    appBuilder.Run(async context =>
                    {
                        context.Response.StatusCode = 500;
                        await context.Response.WriteAsync("An Unexpected Fault Happened. Please try later");
                    });
                });
            }

            app.UseResponseCaching();

            //app.UseHttpCacheHeaders();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using Songify.Simple.DAL;

namespace Songify.Simple
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
            services.AddDbContext<SongifyDbContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("DefaultConnection"),
                    conf =>
                    {
                        conf.MigrationsAssembly(typeof(Startup).Assembly.FullName);
                    }));

            services.Scan(scan => scan.FromAssemblyOf<Startup>()
                .AddClasses(@class => @class.AssignableTo<IRepository>())
                .AsImplementedInterfaces());

            services.AddControllers(options =>
                {
                    options.Conventions.Add(new RouteTokenTransformerConvention(new SlugifyParametersTransformer()));
                })
                .AddNewtonsoftJson(x =>
                {
                    x.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
                    x.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
                    x.SerializerSettings.NullValueHandling = NullValueHandling.Ignore;
                    x.SerializerSettings.Converters.Add(new StringEnumConverter());
                })

                .ConfigureApiBehaviorOptions(setupAction =>
                    setupAction.InvalidModelStateResponseFactory = context =>
                    {
                        var problemDetailsFactory =
                            context.HttpContext.RequestServices.GetRequiredService<ProblemDetailsFactory>();
                        var problemDetails =
                            problemDetailsFactory.CreateValidationProblemDetails(context.HttpContext,
                                context.ModelState);

                        problemDetails.Detail = "See the error field for details";
                        problemDetails.Instance = context.HttpContext.Request.Path;

                        var actionExecutingContext = context as ActionExecutingContext;
                        if ((context.ModelState.ErrorCount > 0) && (actionExecutingContext?.ActionArguments.Count() ==
                                                                    context.ActionDescriptor.Parameters.Count))
                        {
                            problemDetails.Type = "http://songify.com/modelvalidationproblem";
                            problemDetails.Status = StatusCodes.Status422UnprocessableEntity;
                            problemDetails.Title = "One or more validation errors occurred";

                            return new UnprocessableEntityObjectResult(problemDetails)
                            {
                                ContentTypes = {"application/problem+json"}
                            };
                        }

                        problemDetails.Status = StatusCodes.Status400BadRequest;
                        problemDetails.Title = "One or more errors on input data occurred";

                        return new BadRequestObjectResult(problemDetails)
                        {
                            ContentTypes = {"application/problem+json"}
                        };
                    });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "Songify.Simple", 
                    Version = "v1",
                    Description = "Dokumentacja api Songify",
                    TermsOfService = new Uri("https://songify.com/termsOfServie.pdf", UriKind.Absolute),
                    Contact = new OpenApiContact
                    {
                        Name = "Songify Support",
                        Email = "project.codingit@gmail.com",
                        Url = new Uri("https://projectcoding-it.com", UriKind.Absolute)
                    }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAutoMapper(typeof(Startup));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Songify.Simple v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

using Contracts;
using Entities;
using LoggerService;
using Microsoft.EntityFrameworkCore;
using Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;

namespace CompanyEmployees.Extensions
{
    public static class ServiceExtensions
    {
        public static void ConfigureCors(this IServiceCollection services)
        {
            services.AddCors(options =>
            {
                options.AddPolicy("CorsPolicy", builder =>
                {
                    builder.AllowAnyOrigin()
                           .AllowAnyMethod()
                           .AllowAnyHeader();
                });
            });
        }

        public static void ConfigureIISIntegration(this IServiceCollection services)
        {
            services.Configure<IISOptions>(Options =>
            {

            });
        }

        public static void ConfigureLoggerService(this IServiceCollection services)
        {
            services.AddSingleton<ILoggerManager, LoggerManager>();
        }

        public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<RepositoryContext>(opts =>
                      opts.UseSqlServer(configuration.GetConnectionString("sqlConnection"),
                      b => b.MigrationsAssembly("CompanyEmployees")));

        }

        public static void ConfigureRepositoryManager(this IServiceCollection services)
        {
            services.AddScoped<IRepositoryManager, RepositoryManager>();
        }

        public static IMvcBuilder AddCustomCSVFormatter(this IMvcBuilder builder){
            return builder.AddMvcOptions(config => config.OutputFormatters.Add(new CsvOutputFormatter()));
        }

        // public static void AddCustomMediaTypes(this IServiceCollection services)
        // {
        //    services.Configure<MvcOptions>(config =>
        //    {
        //     var newtonsoftJsonOutputFormatter = config.OutputFormatters
        //         .ofType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();


        //     if(newtonsoftJsonOutputFormatter != null)
        //         {
        //         newtonsoftJsonOutputFormatter
        //             .SupportedMediaTypes
        //             .Add("application/vnd.codemaze.hateoas+json");
        //         }  

        //     var xmlOutputFormatter = config.OutputFormatters
        //         .ofType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

        //      if(xmlOutputFormatter != null)
        //         {
        //             xmlOutputFormatter
        //                .SupportedMediaTypes
        //                .Add("application/vnd.codemaze.hateoas+xml");
        //         }   

        //    });
        // }


                public static void AddCustomMediaTypes(this IServiceCollection services)
        {
            services.Configure<MvcOptions>(config =>
            {
                var newtonsoftJsonOutputFormatter = config.OutputFormatters
                      .OfType<NewtonsoftJsonOutputFormatter>()?.FirstOrDefault();

                if (newtonsoftJsonOutputFormatter != null)
                {
                    newtonsoftJsonOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+json");
                }

                var xmlOutputFormatter = config.OutputFormatters
                      .OfType<XmlDataContractSerializerOutputFormatter>()?.FirstOrDefault();

                if (xmlOutputFormatter != null)
                {
                    xmlOutputFormatter.SupportedMediaTypes.Add("application/vnd.codemaze.hateoas+xml");
                }
            });
        }
    }
}
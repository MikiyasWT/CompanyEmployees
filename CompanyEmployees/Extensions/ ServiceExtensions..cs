using Contracts;
using LoggerService;
using Microsoft.EntityFrameworkCore;


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


}

}
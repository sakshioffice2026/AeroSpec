using AeroSpec.Business.Contracts;
using AeroSpec.Business.Services;
using AeroSpec.Database;
using AeroSpec.Repositories.Contracts;
using AeroSpec.Repositories.Repositories;
using Microsoft.EntityFrameworkCore;

namespace AeroSpec.Web.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddAeroSpecDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        return services;
    }

    public static IServiceCollection AddAeroSpecRepositories(this IServiceCollection services)
    {
        services.AddScoped<IFanSizeRepository, FanSizeRepository>();
        services.AddScoped<IFanTypeRepository, FanTypeRepository>();
        services.AddScoped<IPerformanceDataRepository, PerformanceDataRepository>();
        services.AddScoped<IFanSelectionRepository, FanSelectionRepository>();
        services.AddScoped<IUnitOfWork, UnitOfWork>();

        return services;
    }

    public static IServiceCollection AddAeroSpecBusinessServices(this IServiceCollection services)
    {
        services.AddScoped<IFanCalculationService, FanCalculationService>();
        services.AddScoped<IFanEvaluationService, FanEvaluationService>();
        services.AddScoped<IQuoteService, QuoteService>();
        services.AddScoped<IFanSelectionService, FanSelectionService>();

        return services;
    }
}
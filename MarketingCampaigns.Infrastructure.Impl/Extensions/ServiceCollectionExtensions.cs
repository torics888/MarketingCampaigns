using MarketingCampaigns.Infrastructure.Impl.Data;
using MarketingCampaigns.Infrastructure.Impl.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Repositories;

namespace MarketingCampaigns.Infrastructure.Impl.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<CampaignDbContext>(options => options.UseSqlite(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ICampaignRepository, CampaignRepository>();

        return services;
    }
}

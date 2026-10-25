
using MarketingCampaigns.Library.Contracts.Services;
using MarketingCampaigns.Library.Impl.Services;
using Microsoft.Extensions.DependencyInjection;

namespace MarketingCampaigns.Library.Impl.Extensions;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddLibraryServices(this IServiceCollection services)
    {
        services.AddScoped<ICampaignService, CampaignService>();

        return services;
    }
}

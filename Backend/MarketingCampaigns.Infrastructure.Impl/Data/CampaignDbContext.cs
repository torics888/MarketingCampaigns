using MarketingCampaigns.Infrastructure.Contracts.Entities;
using Microsoft.EntityFrameworkCore;

namespace MarketingCampaigns.Infrastructure.Impl.Data;

public class CampaignDbContext(
    DbContextOptions<CampaignDbContext> options) : DbContext(options)
{
    public DbSet<Campaign> Campaigns => Set<Campaign>();
}
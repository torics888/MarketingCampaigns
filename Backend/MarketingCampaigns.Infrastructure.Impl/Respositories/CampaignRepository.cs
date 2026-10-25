using MarketingCampaigns.Infrastructure.Impl.Data;
using MarketingCampaigns.Infrastructure.Contracts.Entities;
using MarketingCampaigns.Library.Contracts.Dtos;
using MarketingCampaigns.Library.Contracts.Enums;
using Microsoft.EntityFrameworkCore;
using Repositories;

namespace MarketingCampaigns.Infrastructure.Impl.Repositories;

public class CampaignRepository(CampaignDbContext context) : ICampaignRepository
{
    public async Task<bool> CreateCampaignAsync(CampaignRequestDto dto)
    {
        var campaign = new Campaign
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            TotalAmount = dto.TotalAmount,
            ConsumedAmount = 0,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = dto.Status.ToString()
        };

        context.Campaigns.Add(campaign);
        await context.SaveChangesAsync();

        return true;

    }

    public async Task<UpdateCampaignResult> UpdateCampaignAsync(Guid id, string status)
    {
        var campaign = await context.Campaigns.FindAsync(id);

        if (campaign == null)
            return UpdateCampaignResult.NotFound;

        if(campaign.EndDate < DateTime.UtcNow)
            return UpdateCampaignResult.Expired;

        if ( campaign.ConsumedAmount >= campaign.TotalAmount)
            return UpdateCampaignResult.AmountExceeded;

        if (campaign.Status == CampaignStatus.Finalitzada.ToString())
            return UpdateCampaignResult.Finalized;

        campaign.Status = status;

        await context.SaveChangesAsync();

        return UpdateCampaignResult.Updated;
    }


    public async Task<ConsumeAmountResult> ConsumeAmountAsync(Guid id, decimal amount)
    {
        var campaign = await context.Campaigns.FindAsync(id);

        if (campaign == null)
            return ConsumeAmountResult.NotFound;

        if (campaign.Status != CampaignStatus.Activa.ToString().ToLowerInvariant())
            return ConsumeAmountResult.CampaignNotActive;

        var now = DateTime.UtcNow;

        if (now < campaign.StartDate || now > campaign.EndDate)
            return ConsumeAmountResult.CampaignExpired;

        if (campaign.ConsumedAmount + amount > campaign.TotalAmount)
            return ConsumeAmountResult.AmountExceeded;

        campaign.ConsumedAmount += amount;

        if (campaign.ConsumedAmount == campaign.TotalAmount)
            campaign.Status = CampaignStatus.Finalitzada.ToString();

        await context.SaveChangesAsync();

        return ConsumeAmountResult.Consumed;
    }


    public async Task<IEnumerable<Campaign>> GetCampaignsAsync(string? status)
    {
        var query = context.Campaigns.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
        {
            query = query.Where(c => c.Status.ToLower() == status.ToLower());
        }

        return await query.ToListAsync();
    }
}
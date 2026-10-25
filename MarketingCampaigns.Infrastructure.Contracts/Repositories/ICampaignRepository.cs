using MarketingCampaigns.Infrastructure.Contracts.Entities;
using MarketingCampaigns.Library.Contracts.Dtos;
using MarketingCampaigns.Library.Contracts.Enums;

namespace Repositories;

public interface ICampaignRepository
{

    Task<IEnumerable<Campaign>> GetCampaignsAsync(string? status);

    Task<bool> CreateCampaignAsync(CampaignRequestDto dto);

    Task<UpdateCampaignResult> UpdateCampaignAsync(Guid id, string status);

    Task<ConsumeAmountResult> ConsumeAmountAsync(Guid id, decimal amount);
}
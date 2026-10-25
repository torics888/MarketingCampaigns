using MarketingCampaigns.Library.Contracts.Dtos;

namespace MarketingCampaigns.Library.Contracts.Services;

public interface ICampaignService
{
    Task<IEnumerable<CampaignResponseDto>> GetCampaigns(string? status);

    Task<ServiceResult<bool>> CreateCampaign(CreateCampaignRequestDto dto);

    Task<ServiceResult<bool>> UpdateCampaign(Guid id, CampaignUpdateStatusDto status);

    Task<ServiceResult<bool>> ConsumeAmount(Guid id, ConsumeAmountRequestDto dto);


}
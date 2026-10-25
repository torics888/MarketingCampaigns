
namespace MarketingCampaigns.Library.Contracts.Enums;

public enum CampaignStatus
{
    Esborrany,
    Activa,
    Pausada,
    Finalitzada
}

public enum UpdateCampaignResult
{
    Updated,
    NotFound,
    Finalized,
    Expired,
    AmountExceeded
}

public enum ConsumeAmountResult
{
    Consumed,
    NotFound,
    CampaignNotActive,
    CampaignExpired,
    InvalidAmount,
    AmountExceeded
}
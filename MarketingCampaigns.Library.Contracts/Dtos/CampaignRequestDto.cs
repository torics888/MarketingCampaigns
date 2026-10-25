using MarketingCampaigns.Library.Contracts.Enums;


namespace MarketingCampaigns.Library.Contracts.Dtos;

public class CampaignRequestDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal ConsumedAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public CampaignStatus Status { get; set; }
}

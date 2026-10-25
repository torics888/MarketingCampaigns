

namespace MarketingCampaigns.Infrastructure.Contracts.Entities;
public class Campaign
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal ConsumedAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;
}
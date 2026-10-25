using MarketingCampaigns.Library.Contracts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingCampaigns.Library.Contracts.Dtos;

public class CampaignResponseDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public decimal TotalAmount { get; set; }

    public decimal ConsumedAmount { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public string Status { get; set; } = string.Empty;
}

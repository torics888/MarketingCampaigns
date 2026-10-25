using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MarketingCampaigns.Library.Contracts.Dtos;

public class CreateCampaignRequestDto
{

    public required string Name { get; set; } = string.Empty;
    public required decimal TotalAmount { get; set; } = 0;
    public required DateTime StartDate { get; set; } = DateTime.MinValue;
    public required DateTime EndDate { get; set; } = DateTime.MinValue;


}

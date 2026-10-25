using MarketingCampaigns.Library.Contracts.Dtos;
using MarketingCampaigns.Library.Contracts.Enums;
using MarketingCampaigns.Library.Contracts.Services;
using Repositories;

namespace MarketingCampaigns.Library.Impl.Services;

public class CampaignService(
    ICampaignRepository repository) : ICampaignService
{


    public async Task<IEnumerable<CampaignResponseDto>> GetCampaigns(string? status)
    {
        if (!string.IsNullOrWhiteSpace(status))
        {
            var validStatus = Enum.GetNames<CampaignStatus>().FirstOrDefault(s => string.Equals(s, status, StringComparison.OrdinalIgnoreCase));

            if (validStatus is null)
            {
                throw new ArgumentException("Invalid campaign status");
            }

            status = validStatus;
        }

        var campaigns = await repository.GetCampaignsAsync(status);

        return campaigns.Select(c => new CampaignResponseDto
        {
            Id = c.Id,
            Name = c.Name,
            TotalAmount = c.TotalAmount,
            ConsumedAmount = c.ConsumedAmount,
            StartDate = c.StartDate,
            EndDate = c.EndDate,
            Status = c.Status
        });
    }



    public async Task<ServiceResult<bool>> CreateCampaign(CreateCampaignRequestDto dto)
    {

        if (dto.StartDate >= dto.EndDate)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Start date must be before end date."
            };
        }

        if (dto.TotalAmount <= 0)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Total amount must be a positive value."
            };
        }

        var campaignRequest = new CampaignRequestDto
        {
            Id = Guid.NewGuid(),
            Name = dto.Name,
            TotalAmount = dto.TotalAmount,
            ConsumedAmount = 0,
            StartDate = dto.StartDate,
            EndDate = dto.EndDate,
            Status = CampaignStatus.Esborrany
        };

        var campaign = await repository.CreateCampaignAsync(campaignRequest);

        if (campaign) return new ServiceResult<bool> { Success = true, StatusCode = 200, Message = "Campaign created successfully." };

        return new ServiceResult<bool> { Success = false, StatusCode = 500, Message = "Failed to create campaign." };
    }

    public async Task<ServiceResult<bool>> UpdateCampaign(Guid id, CampaignUpdateStatusDto status)
    {
        if ((status.Status.ToLowerInvariant() != CampaignStatus.Activa.ToString().ToLowerInvariant() && status.Status.ToLowerInvariant() != CampaignStatus.Pausada.ToString().ToLowerInvariant()))
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Invalid campaign status"
            };
        }

        //actualitzar campanya
        var updated = await repository.UpdateCampaignAsync(id, status.Status);

        if (updated == UpdateCampaignResult.Updated)
        {
            return new ServiceResult<bool>
            {
                Success = true,
                StatusCode = 200,
                Message = "Campaign updated successfully"
            };
        }

        if(updated == UpdateCampaignResult.Expired)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Cannot update expired campaign"
            };
        }

        if (updated == UpdateCampaignResult.AmountExceeded)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Cannot update campaign with exceeded amount"
            };
        }

        if (updated == UpdateCampaignResult.NotFound)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 404,
                Message = "Campaign not found"
            };
        }

        if (updated == UpdateCampaignResult.Finalized)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Cannot update finalized campaign"
            };
        }

        return new ServiceResult<bool>
        {
            Success = false,
            StatusCode = 500,
            Message = "Error to update campaign"
        };

    }

    public async Task<ServiceResult<bool>> ConsumeAmount(Guid id, ConsumeAmountRequestDto request)
    {
        if (request.Amount <= 0)
        {
            return new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 400,
                Message = "Amount must be greater than zero."
            };
        }

        var result = await repository.ConsumeAmountAsync(id, request.Amount);

        return result switch
        {
            ConsumeAmountResult.Consumed => new ServiceResult<bool>
            {
                Success = true,
                StatusCode = 200,
                Message = "Amount consumed successfully."
            },

            ConsumeAmountResult.NotFound => new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 404,
                Message = "Campaign not found."
            },

            ConsumeAmountResult.CampaignNotActive => new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 409,
                Message = "Campaign must be active to consume amount"
            },

            ConsumeAmountResult.CampaignExpired => new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 409,
                Message = "Campaign is outside its validity period"
            },

            ConsumeAmountResult.AmountExceeded => new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 409,
                Message = "Consumption exceeds the remaining amount"
            },

            _ => new ServiceResult<bool>
            {
                Success = false,
                StatusCode = 500,
                Message = "Error consuming Amount"
            }
        };
    }

}

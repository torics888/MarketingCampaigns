using MarketingCampaigns.Controllers;
using MarketingCampaigns.Library.Contracts.Dtos;
using MarketingCampaigns.Library.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;

namespace MarketingCampaigns.Tests;

public class CampaignsTests
{
    private readonly StubCampaignService _service = new();
    private CampaignsController CreateController() =>
        new(NullLogger<CampaignsController>.Instance, _service);

    [Fact]
    public async Task GetCampaigns_ReturnsOkAndCampaignList()
    {
        var expected = new[]
        {
            new CampaignResponseDto { Id = Guid.NewGuid(), Name = "Campanya test", Status = "Activa" }
        };
        _service.Campaigns = expected;
        var result = await CreateController().GetCampaigns("Activa");
        var ok = Assert.IsType<OkObjectResult>(result);
        Assert.Same(expected, ok.Value);
        Assert.Equal("Activa", _service.LastStatusFilter);
    }

    [Fact]
    public async Task CreateCampaign_ReturnsServiceStatusCode()
    {
        _service.CreateResult = new ServiceResult<bool> { Success = true, StatusCode = 200, Message = "Campaign created successfully." };
        var dto = new CreateCampaignRequestDto
        {
            Name = "Campanya estiu",
            TotalAmount = 1000m,
            StartDate = DateTime.UtcNow,
            EndDate = DateTime.UtcNow.AddDays(10)
        };
        var result = await CreateController().CreateCampaign(dto);
        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(200, status.StatusCode);
        Assert.Same(_service.CreateResult, status.Value);
        Assert.Same(dto, _service.LastCreateDto);
    }

    [Fact]
    public async Task UpdateCampaign_ReturnsServiceStatusCode()
    {
        var id = Guid.NewGuid();
        var dto = new CampaignUpdateStatusDto { Status = "Pausada" };
        _service.UpdateResult = new ServiceResult<bool> { Success = false, StatusCode = 404, Message = "Campaign not found" };
        var result = await CreateController().UpdateCampaign(id, dto);
        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(404, status.StatusCode);
        Assert.Same(dto, _service.LastStatusDto);
        Assert.Equal(id, _service.LastCampaignId);
    }

    [Fact]
    public async Task ConsumeAmount_ReturnsServiceStatusCode()
    {
        var id = Guid.NewGuid();
        var dto = new ConsumeAmountRequestDto { Amount = 25.50m };
        _service.ConsumeResult = new ServiceResult<bool> { Success = false, StatusCode = 409, Message = "Consumption exceeds the remaining amount" };
        var result = await CreateController().ConsumeAmount(id, dto);
        var status = Assert.IsType<ObjectResult>(result);
        Assert.Equal(409, status.StatusCode);
        Assert.Same(dto, _service.LastConsumeDto);
        Assert.Equal(id, _service.LastCampaignId);
    }

    private sealed class StubCampaignService : ICampaignService
    {
        public IEnumerable<CampaignResponseDto> Campaigns { get; set; } = Array.Empty<CampaignResponseDto>();
        public ServiceResult<bool> CreateResult { get; set; } = new() { Success = true, StatusCode = 200 };
        public ServiceResult<bool> UpdateResult { get; set; } = new() { Success = true, StatusCode = 200 };
        public ServiceResult<bool> ConsumeResult { get; set; } = new() { Success = true, StatusCode = 200 };
        public string? LastStatusFilter { get; private set; }
        public CreateCampaignRequestDto? LastCreateDto { get; private set; }
        public CampaignUpdateStatusDto? LastStatusDto { get; private set; }
        public ConsumeAmountRequestDto? LastConsumeDto { get; private set; }
        public Guid LastCampaignId { get; private set; }

        public Task<IEnumerable<CampaignResponseDto>> GetCampaigns(string? status)
        {
            LastStatusFilter = status;
            return Task.FromResult(Campaigns);
        }
        public Task<ServiceResult<bool>> CreateCampaign(CreateCampaignRequestDto dto)
        {
            LastCreateDto = dto; return Task.FromResult(CreateResult);
        }
        public Task<ServiceResult<bool>> UpdateCampaign(Guid id, CampaignUpdateStatusDto status)
        {
            LastCampaignId = id; LastStatusDto = status; return Task.FromResult(UpdateResult);
        }
        public Task<ServiceResult<bool>> ConsumeAmount(Guid id, ConsumeAmountRequestDto dto)
        {
            LastCampaignId = id; LastConsumeDto = dto; return Task.FromResult(ConsumeResult);
        }
    }
}

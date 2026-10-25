using MarketingCampaigns.Library.Contracts.Dtos;
using MarketingCampaigns.Library.Contracts.Services;
using Microsoft.AspNetCore.Mvc;

namespace MarketingCampaigns.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignsController(
        ILogger<CampaignsController> logger,
        ICampaignService service

        ) : ControllerBase
    {
        [HttpGet]
        public async Task<IActionResult> GetCampaigns([FromQuery] string? status)
        {

            var resultado = await service.GetCampaigns(status);

            return Ok(resultado);
        }


        [HttpPost]
        public async Task<IActionResult> CreateCampaign(CreateCampaignRequestDto dto)
        {
            
            var resultado = await service.CreateCampaign(dto);

            return StatusCode(resultado.StatusCode, resultado);

        }


        [HttpPut]
        [Route("{id}/status")]
        public async Task<IActionResult> UpdateCampaign(Guid id, CampaignUpdateStatusDto dto)
        {
       
            var resultado = await service.UpdateCampaign(id, dto);

            return StatusCode(resultado.StatusCode, resultado);

        }

        [HttpPost("{id}/consume")]
        public async Task<IActionResult> ConsumeAmount(Guid id, ConsumeAmountRequestDto dto)
        {
            var result = await service.ConsumeAmount(id, dto);

            return StatusCode(result.StatusCode, result);
        }

    }
}

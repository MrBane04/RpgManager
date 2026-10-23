using Microsoft.AspNetCore.Mvc;
using RpgManager.Core.Services;
using RpgManager.DTOs;
using RpgManager.Core.Models;

namespace RpgManager.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CampaignsController : ControllerBase
    {
        private readonly CampaignService _campaignService;

        public CampaignsController(CampaignService campaignService)
        {
            this._campaignService = campaignService;
        }

        [HttpPost]
        public IActionResult CreateCampaign([FromBody] CreateCampaignRequest request)
        {
            if (!Enum.TryParse<CampaignStatus>(request.Status,true, out var status) || !Enum.IsDefined(status))
            {
                return BadRequest("Nieprawidłowy status kampanii.");
            }
            OperationResult result = _campaignService.CreateCampaign(request.Name, request.System, status);
            if(result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Message);
            }
        }
    }
}

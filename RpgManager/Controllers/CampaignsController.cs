using Microsoft.AspNetCore.Mvc;
using RpgManager.Core.Services;
using RpgManager.DTOs;

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
            OperationResult result = _campaignService.CreateCampaign(request.Name);
            if(result.Success)
            {
                return Ok(result);
            }
            else
            {
                return BadRequest(result.Text);
            }
        }
    }
}

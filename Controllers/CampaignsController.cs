using RpgManager.Services;

namespace RpgManager.Controllers
{
    public class CampaignsController
    {
        private readonly CampaignService _campaignService;

        public CampaignsController(CampaignService campaignService)
        {
            this._campaignService = campaignService;
        }
    }
}

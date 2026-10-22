using RpgManager.Core.Models;
namespace RpgManager.Core.Services
{
    public class CampaignService
    {
        private List<Campaign> campaigns = new List<Campaign>();
        public OperationResult CreateCampaign(string name, string system,string status)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(system))
            {
                OperationResult resultFail = new OperationResult(false, "Podaj poprawną wartość.");
                return resultFail;
            }
            else
            {
                Campaign campaign = new Campaign(name, system, status);
                campaigns.Add(campaign);
                OperationResult resultSuccess = new OperationResult(true, "Sukces");
                return resultSuccess;
            }
        }

        public IReadOnlyList<Campaign> GetCampaigns()
        {
            return campaigns;
        }
    }
}

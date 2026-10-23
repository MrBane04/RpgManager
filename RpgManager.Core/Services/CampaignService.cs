using RpgManager.Core.Models;
namespace RpgManager.Core.Services
{
    public class CampaignService
    {
        private List<Campaign> campaigns = new List<Campaign>();
        public OperationResult CreateCampaign(string name, string system, CampaignStatus status)
        {
            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(system) || !Enum.IsDefined(status))
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

        public OperationResult DeleteCampaign(int id)
        {
            Campaign? foundCampaign = campaigns.FirstOrDefault(c => c.Id == id);
            if(foundCampaign != null)
            {
                campaigns.Remove(foundCampaign);
                OperationResult resultSuccess = new OperationResult(true,"Poprawnie usunięto kampanię.");
                return resultSuccess;
            }
            else
            {
                OperationResult resultFailed = new OperationResult(false, "Operacja usunięcia kampani się nie powiodła, nie znaleziono kampanii o podanym ID");
                return resultFailed;
            }
        }
    }
}

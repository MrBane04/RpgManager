using RpgManager.Core.Models;
using RpgManager.Core.Data;
using Microsoft.EntityFrameworkCore;
namespace RpgManager.Core.Services
{
    public class CampaignService
    {
        private readonly RpgManagerDbContext _dbContext;

        public CampaignService(RpgManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

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
                _dbContext.Campaigns.Add(campaign);
                _dbContext.SaveChanges();
                OperationResult resultSuccess = new OperationResult(true, "Sukces");
                return resultSuccess;
            }
        }

        public IReadOnlyList<Campaign> GetCampaigns()
        {
            return _dbContext.Campaigns.ToList();
        }

        public OperationResult DeleteCampaign(int id)
        {
            Campaign? foundCampaign = _dbContext.Campaigns.FirstOrDefault(c => c.Id == id);
            if(foundCampaign != null)
            {
                _dbContext.Campaigns.Remove(foundCampaign);
                _dbContext.SaveChanges();
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

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
                OperationResult resultFail = new OperationResult(false, "Nazwa, system lub status jest niepoprawna.");
                return resultFail;
            }
            else
            {
                Campaign campaign = new Campaign(name, system, status);
                campaign.Note = new Note();
                try
                {
                    _dbContext.Campaigns.Add(campaign);
                    _dbContext.SaveChanges();
                }
                catch(DbUpdateException)
                {
                    
                    return new OperationResult(false, "Nie udało się zapisać kampanii. Spróbuj ponownie");
                }
                OperationResult resultSuccess = new OperationResult(true, "Sukces");
                return resultSuccess;
            }
        }

        public IReadOnlyList<Campaign> GetCampaigns()
        {
            return _dbContext.Campaigns.ToList();
        }

        public OperationResult EditCampaign(int id,string name,string system, CampaignStatus status)
        {
            Campaign? foundCampaign = _dbContext.Campaigns.FirstOrDefault(c => c.Id == id);
            if(foundCampaign == null)
            {
                OperationResult resultFail = new OperationResult(false, "Kampania o podanym id nie istnieje.");
                return resultFail;
            }
            else if(string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(system) || !Enum.IsDefined(status))
            {
                OperationResult resultFailArguments = new OperationResult(false, "Name, system lub status jest niepoprawny.");
                return resultFailArguments;
            }
            else
            {
                try
                {
                    foundCampaign.Name = name;
                    foundCampaign.System = system;
                    foundCampaign.Status = status;
                    _dbContext.SaveChanges();

                }
                catch(DbUpdateException)
                {
                    return new OperationResult(false, "Nie udało się zaktualizować kampanii.");
                }
                OperationResult resultSuccess = new OperationResult(true, "Poprawnie dokonano edycji kampanii.");
                return resultSuccess;
            }
        }

        public OperationResult DeleteCampaign(int id)
        {
            Campaign? foundCampaign = _dbContext.Campaigns.FirstOrDefault(c => c.Id == id);
            if(foundCampaign != null)
            {
                try
                {
                    _dbContext.Campaigns.Remove(foundCampaign);
                    _dbContext.SaveChanges();
                }
                catch(DbUpdateException)
                {
                    return new OperationResult(false, "Nie udało się usunąć kampanii.");
                }
                
                OperationResult resultSuccess = new OperationResult(true,"Poprawnie usunięto kampanię.");
                return resultSuccess;
            }
            else
            {
                OperationResult resultFailed = new OperationResult(false, "Operacja usunięcia kampani się nie powiodła, nie znaleziono kampanii o podanym ID");
                return resultFailed;
            }
        }

        public bool CampaignExists (int id)
        {
            return _dbContext.Campaigns.Any(c => c.Id == id);
        }
    }
}

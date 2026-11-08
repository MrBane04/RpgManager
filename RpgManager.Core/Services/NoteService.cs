using RpgManager.Core.Models;
using RpgManager.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace RpgManager.Core.Services
{
    public class NoteService
    {
        private readonly RpgManagerDbContext _dbContext;

        public Note? GetNote(int campaignId)
        {
            return _dbContext.Notes.FirstOrDefault(n => n.CampaignId == campaignId);
        }

        public OperationResult UpdateNote(int campaignId, string content)
        {
            Note? foundNote = _dbContext.Notes.FirstOrDefault(n => n.CampaignId == campaignId);
            if(foundNote == null)
            {
                return new OperationResult(false, "Nie znaleziono notatki.");
            }
            if (content == null)
            {
                return new OperationResult(false, "Treść notatki nie może być null.");
            }
            else
            {
                try
                {
                    foundNote.Content = content;
                    foundNote.UpdatedAt = DateTime.UtcNow;
                    _dbContext.SaveChanges();
                }
                catch(ArgumentException ex)
                {
                    return new OperationResult(false, ex.Message);
                }
                catch (DbUpdateException)
                {
                    return new OperationResult(false, "Edycja nie powiodła się");
                }
                return new OperationResult(true, "Edycja powiodła się.");
            }
        }
        public NoteService(RpgManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

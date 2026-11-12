using RpgManager.Core.Models;
using RpgManager.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace RpgManager.Core.Services
{
    public class SessionService
    {
        private readonly RpgManagerDbContext _dbContext;

        public SessionService(RpgManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public OperationResult CreateSession(int campaignId, string title, string? description,
            DateTime sessionDate, int? durationTime,SessionStatus status)
        { 
            if(!_dbContext.Campaigns.Any(c => c.Id == campaignId))
            {
                return new OperationResult(false, "Kampania nie istnieje");
            }
            else
            {
                if (!Enum.IsDefined(status))
                {
                    return new OperationResult(false, "Nieprawidłowy status sesji.");
                }
                try
                {
                    Session session = new Session(campaignId,title,description,sessionDate,durationTime,status);
                    _dbContext.Sessions.Add(session);
                    _dbContext.SaveChanges();
                }
                catch(DbUpdateException)
                {
                    return new OperationResult(false, "Nie udało się dodać sesji.");
                }
                catch(ArgumentException ex)
                {
                    return new OperationResult(false, ex.Message);
                }
                return new OperationResult(true, "Udało się dodać sesję.");
            }
        }

        public IReadOnlyList<Session> GetSessions(int campaignId, int page, int pageSize)
        {
            if(page <1)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }
            if(pageSize<1)
            {
                throw new ArgumentOutOfRangeException(nameof(pageSize));
            }
            return _dbContext.Sessions
                .Where(s => s.CampaignId == campaignId)
                .OrderByDescending(s => s.SessionDate)
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToList();
        }
        public int CountSessions(int campaignId)
        {
            return _dbContext.Sessions
                .Count(s=>s.CampaignId == campaignId);
        }

        public IReadOnlyList<Session> GetSessions(int campaignId)
        {
            return _dbContext.Sessions
                .Where(s => s.CampaignId == campaignId)
                .OrderByDescending( s => s.SessionDate)
                .ToList();
        }

        public Session? GetSession(int campaignId, int sessionId)
        {
            return _dbContext.Sessions.FirstOrDefault(s => s.Id == sessionId && s.CampaignId == campaignId);
        }

        public OperationResult DeleteSession(int campaignId, int sessionId)
        {
            Session? foundSession = _dbContext.Sessions.FirstOrDefault(s => s.Id == sessionId && s.CampaignId == campaignId);
            if (foundSession == null)
            {
                return new OperationResult(false, "Nie znaleziono sesji w tej kampanii.");
            }
            else
            {
                try
                {
                    _dbContext.Sessions.Remove(foundSession);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    return new OperationResult(false, "Nie udało się usunąć sesji.");
                }
                return new OperationResult(true, "Poprawnie usunięto sesję.");
            }
        }

        public OperationResult EditSession(int campaignId,int sessionId, int? durationTime,
            string title, string? description,
            DateTime sessionDate, SessionStatus status)
        {
            Session? foundSession = _dbContext.Sessions.FirstOrDefault(s => s.Id == sessionId && s.CampaignId == campaignId);
            if(foundSession == null)
            {
                return new OperationResult(false, "Sesja o podanym id nie istnieje.");
            }
            else
            {
                if (string.IsNullOrWhiteSpace(title))
                {
                    return new OperationResult(false, "Tytuł sesji jest wymagany.");
                }
                if (durationTime.HasValue && durationTime.Value<=0)
                {
                    return new OperationResult(false, "Czas trwania musi być większy od zera.");
                }
                if(!Enum.IsDefined(status))
                {
                    return new OperationResult(false, "Nieprawidłowy status sesji.");
                }
                try
                {
                    foundSession.Title = title;
                    foundSession.Description = description;
                    foundSession.SessionDate = sessionDate;
                    foundSession.DurationMinutes = durationTime;
                    foundSession.Status = status;
                    _dbContext.SaveChanges();
                }
                catch(ArgumentException)
                {
                    return new OperationResult(false, "Edycja nie powiodła się");
                }
                catch(DbUpdateException)
                {
                    return new OperationResult(false, "Edycja nie powiodła się");
                }
                return new OperationResult(true, "Edycja powiodła się.");
            }
        }

    }
}

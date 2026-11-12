using RpgManager.Core.Models;
using RpgManager.Core.Data;
using Microsoft.EntityFrameworkCore;

namespace RpgManager.Core.Services
{ 
    public class CharacterService
    {
        private readonly RpgManagerDbContext _dbContext;

        public OperationResult CreateCharacter(
            int campaignId, string name, int age,
            string? race, string? characterClass,
            int level, string? description)
        {
            if(!_dbContext.Campaigns.Any(c => c.Id == campaignId))
            {
                return new OperationResult(false, "Kampania nie istnieje.");
            }
            else
            {
                try
                {
                    Character character = new Character(campaignId,name,age,race,characterClass,level,description);
                    _dbContext.Characters.Add(character);
                    _dbContext.SaveChanges();
                }
                catch(DbUpdateException )
                {
                    return new OperationResult(false, "Nie udało się dodać postaci.");
                }
                catch(ArgumentException ex)
                {
                    return new OperationResult(false, ex.Message);
                }
            }
            return new OperationResult(true, "Udało się dodać postać.");
        }

        public IReadOnlyList<Character> GetCharacters(int campaignId,int page,int pageSize)
        {
            if (page < 1 || pageSize < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(page));
            }
            return _dbContext.Characters
                .Where(c => c.CampaignId == campaignId)
                .OrderBy(c => c.Name)
                .Skip((page-1)*pageSize)
                .Take(pageSize)
                .ToList();
        }

        public IReadOnlyList<Character> GetCharacters(int campaignId)
        {
            return _dbContext.Characters
                .Where(c => c.CampaignId == campaignId)
                .OrderBy(c => c.Name)
                .ToList();
        }

        public int CountCharacters(int campaignId)
        {
            return _dbContext.Characters
                .Count(c => c.CampaignId == campaignId);
        }

        public Character? GetCharacter(int campaignId, int characterId)
        {
            return _dbContext.Characters.FirstOrDefault(c => c.Id == characterId && c.CampaignId == campaignId);
        }

        public OperationResult DeleteCharacter(int campaignId, int characterId)
        {
            Character? foundCharacter = _dbContext.Characters.FirstOrDefault(c => c.Id == characterId && c.CampaignId==campaignId);
            if(foundCharacter == null)
            {
                return new OperationResult(false, "Nie znaleziono postaci.");
            }
            else
            {
                try
                {
                    _dbContext.Characters.Remove(foundCharacter);
                    _dbContext.SaveChanges();
                }
                catch (DbUpdateException)
                {
                    return new OperationResult(false, "Nie udało się usunąć postaci.");
                }
                return new OperationResult(true, "Poprawnie usunięto postać.");
            }
        }

        public OperationResult EditCharacter(int campaignId, int characterId,
            string name, int age, string? race,
            string? characterClass, int level,
            string? description)
        {
            Character? foundCharacter = _dbContext.Characters.FirstOrDefault(c => c.Id == characterId && c.CampaignId == campaignId);
            if(foundCharacter == null)
            {
                return new OperationResult(false, "Postać o podanym id nie istnieje.");
            }
            else
            {
                if(string.IsNullOrWhiteSpace(name))
                {
                    return new OperationResult(false, "Nazwa postaci jest wymagana.");
                }
                if(age <= 0)
                {
                    return new OperationResult(false, "wiek postaci musi być większy od zera.");
                }
                try
                {
                    foundCharacter.Name = name;
                    foundCharacter.Age = age;
                    foundCharacter.Race = race;
                    foundCharacter.CharacterClass = characterClass;
                    foundCharacter.Level = level;
                    foundCharacter.Description = description;
                    _dbContext.SaveChanges();
                }
                catch(ArgumentException ex)
                {
                    return new OperationResult(false, ex.Message);
                }
                catch(DbUpdateException)
                {
                    return new OperationResult(false, "Edycja nie powiodła się");
                }
                return new OperationResult(true, "Edycja powiodła się.");
            }
        }

        public CharacterService(RpgManagerDbContext dbContext)
        {
            _dbContext = dbContext;
        }
    }
}

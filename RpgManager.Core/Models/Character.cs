namespace RpgManager.Core.Models;

public class Character
{
    private string _name = string.Empty;
    private int _age = 0;
    private int _level = 1;
    

    public int Level { get => _level;
        set
        {
            if (value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Level), "Poziom musi być większy od zera.");
            }
            else
            {
                _level = value;
            }
        }
    }

    public string? Race {  get; set; }
    public string? CharacterClass { get; set; }
    public int Age { get => _age;
        set
        {
            if(value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(Age),"Wiek musi być większy od zera.");
            }
            else
            {
                _age = value;
            }
        }
    }
    public string Name { get =>  _name;
        set
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Wprowadzono niepoprawną nazwę.");
            }
            else
            {
                _name = value;
            }
        }
    }

    public DateTime CreatedAt { get; set; }
    public string? Description { get; set; }
    public int CampaignId { get; set; }
    public int Id { get; set; }

    public Campaign Campaign { get; set; } = null!;



    public Character(int campaignId,string name,int age,string? race,string? characterClass,int level,string? description)
    {
        this.CampaignId = campaignId;
        this.Name = name;
        this.Age = age;
        this.Race = race;
        this.CharacterClass = characterClass;
        this.Level = level;
        this.Description = description;
    }

    public Character()
    {
    }



    
}

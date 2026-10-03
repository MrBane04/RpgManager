class Character : IAttacker
{
    private int _age;
    private string _name;
    public int Health { get; private set; }
    public string Name { 
        get
        {
            return _name;
        } 
        set
        {
            bool validName = string.IsNullOrWhiteSpace(value);
            if(!validName)
            {
                _name = value;
            }
        } }
    public int Age 
    { 
        get
        {
            return _age;
        }
        set 
        {
            if(value>0)
            {
                _age = value;
            }
            else
            {
                throw new ArgumentException("Wiek postaci musi być większy od 0.");
            }
        } 
    }
    public CharacterClass ClassName { get; set; }
    public Character(string characterName, int characterAge, CharacterClass characterClassName)
    {
        Name = characterName;
        Age = characterAge;
        ClassName = characterClassName;
        Health = 100;
    }

    public virtual void Attack(Character target)
    {
        Console.WriteLine($"{this.Name} cię atakuje.");
        int damage;

        switch(this.ClassName)
        {
            case CharacterClass.Warrior:
                damage = 20;
                break;
            case CharacterClass.Mage:
                damage = 30;
                break;
            case CharacterClass.Rogue:
                damage = 25;
                break;
            default:
                Console.WriteLine("Taka klasa nie istnieje");
                damage = 0;
                break;
        }
        target.TakeDamage(damage);

    }

    public void TakeDamage(int damage)
    {
        if(Health > 0 )
        {
            this.Health = Health-damage;

            if(this.Health < 0)
            {
                this.Health = 0;
            }
              
            Console.WriteLine($"{Name} otrzymał {damage} obrażeń!");
            Console.WriteLine($"Pozostałe HP:{Health}");
            
        }
        else
        {
            Console.WriteLine("Postać nie żyje.");
        }
    }

    public void Heal(int amount)
    {
        int HPAfterHealing = Health + amount;
        if(Health>0)
        {
            if(HPAfterHealing<100)
            {
                this.Health = HPAfterHealing;
            }
            else
            {
                this.Health = 100;
            }
        }
        else
        {
            this.Health = 0;
        }
        
    }

    public void ShowInfo()
    {
        Console.WriteLine($"Imię: {this.Name}\nWiek: {this.Age}\nKlasa: {this.ClassName}\nHP: {this.Health}");
    }
}
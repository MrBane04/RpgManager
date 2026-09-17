class Character
{
    private int _age;
    private int _health;
    public int Health { 
        get
        {
            return _health;
        }
        set
        {
            if(value > 0)
            {
                _health = value;
            }
            else
            {
                _health = 0;
            }
        } }
    public string Name { get; set; }
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
        } 
    }
    public string ClassName { get; set; }
    public Character(string characterName, int characterAge, string characterClassName)
    {
        Name = characterName;
        Age = characterAge;
        ClassName = characterClassName;
        Health = 100;
    }

    public void Attack(Character target)
    {
        Console.WriteLine($"{this.Name} cię atakuje.");
        int damage;

        switch(this.ClassName)
        {
            case "Wojownik":
                damage = 20;
                break;
            case "Mag":
                damage = 30;
                break;
            case "Łotrzyk":
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
        if(Health > 0)
        {
            this.Health = Health-damage;
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
}
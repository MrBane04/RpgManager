class Warrior:Character
{
    public int Strength{get;set;}
    public Warrior(string warriorName, int warriorAge, CharacterClass warriorCharacterClass)
        : base(warriorName,warriorAge,warriorCharacterClass)
    {
        Strength = 10;
    }

    public override void Attack(Character target)
    {
        Console.WriteLine("Wojownik atakuje");
    }
}
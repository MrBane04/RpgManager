class CharacterData
{
    public string Name {get;set;}
    public int Age {get;set;}
    public CharacterClass CharacterClass {get;set;}

    public CharacterData(string name, int age, CharacterClass characterClass)
    {
        Name = name;
        Age = age;
        CharacterClass = characterClass;
    }
}
class CharacterManager
{
    private List<Character> characters = new();

    public void AddCharacter(Character character)
    {
        characters.Add(character);
    }

    public void ShowCharacters()
    {
        foreach(Character character in characters)
        {
            character.ShowInfo();
        }
    }

    public Character? FindCharacter(string name)
    {
                Character? foundCharacter = characters.FirstOrDefault(
                    character => character.Name == name
                );
                if(foundCharacter != null)
                {
                    return foundCharacter;
                }
                else
                {
                    return null;
                }
    }

    public IEnumerable<Character> ShowWarriors()
    {
        IEnumerable<Character> warriorCharacters = characters.Where(character => character.ClassName == CharacterClass.Warrior);
        return warriorCharacters;
    }

    public CharacterStatistics ShowStatistics()
    {
        CharacterStatistics characterStatistisc = new CharacterStatistics();
        int oldestCharacter = characters.Max(character => character.Age);
        int youngestCharacter = characters.Min(character => character.Age);
        double averageCharacterAge = characters.Average(character => character.Age);
        int sumHP = characters.Sum(character => character.Health);
        characterStatistisc.OldestCharacter = oldestCharacter;
        characterStatistisc.YoungestCharacter = youngestCharacter;
        characterStatistisc.AverageCharacterAge = averageCharacterAge;
        characterStatistisc.SumHP = sumHP;
        return characterStatistisc;
    }

    public IEnumerable<IGrouping<CharacterClass, Character>> ShowGroups()
    {
        IEnumerable<IGrouping<CharacterClass, Character>> groupedCharacters = characters.GroupBy(character => character.ClassName);
        return groupedCharacters;
                // foreach(var group in groupedCharacters)
                // {
                //     int groupCount = group.Count();
                //     Console.WriteLine($"{group.Key}-{groupCount}");
                // }
    }

    public bool RemoveCharacter(string name)
    {
        

    Character? foundCharacter = characters.FirstOrDefault(character => character.Name == name);

    if(foundCharacter == null)
    {
        return false;
    }
    else
    {
        characters.Remove(foundCharacter);
        return true;
    }
    }

    public void EditCharacter()
    {
        Console.WriteLine("Podaj nazwę postaci: ");
    string name = Console.ReadLine();
    Character? foundCharacter = characters.FirstOrDefault(character => character.Name == name);

    if(foundCharacter != null)
    {
        Console.WriteLine($"{foundCharacter.Name} - {foundCharacter.ClassName} - {foundCharacter.Age}");
        Console.WriteLine("Co chcesz zmienić?\n1. Imię\n2. Wiek\n3. Klasę\n0. Anuluj");
        int choice;
        bool success = int.TryParse(Console.ReadLine(),out choice);

        if(success)
        {
            switch(choice)
            {
                case 1:
                    Console.WriteLine("Podaj nowe imię: ");
                    string newName = Console.ReadLine();
                    foundCharacter.Name = newName;
                    break;
                case 2:
                    while(true)
                    {
                        Console.WriteLine("Podaj nowy wiek postaci: ");
                        int newAge;
                        bool ageSuccess = int.TryParse(Console.ReadLine(),out newAge);

                        if(ageSuccess && newAge > 0)
                        {
                            foundCharacter.Age = newAge;
                            break;
                        }
                        else
                        {
                            Console.WriteLine("Podaj poprawny wiek");
                        }
                    }
                    break;
                case 3:
                    while(true)
                    {
                        Console.WriteLine("Wybierz nową klasę postaci:\n1. Wojownik\n2. Mag\n3. Łotrzyk");
                        int newClass;
                        bool classSuccess = int.TryParse(Console.ReadLine(),out newClass);

                        if(classSuccess && newClass >=1 && newClass <= 3)
                        {
                            switch(newClass)
                            {
                                case 1:
                                    foundCharacter.ClassName = CharacterClass.Warrior;
                                    break;
                                case 2:
                                    foundCharacter.ClassName = CharacterClass.Mage;
                                    break;
                                case 3:
                                    foundCharacter.ClassName = CharacterClass.Rogue;
                                    break;
                                default:
                                    Console.WriteLine("Wybierz poprawną klasę.");
                                    break;
                            }
                            break;
                        }

                    }
                    break;
                case 0:
                    break;
                default:
                    Console.WriteLine("Wybierz poprawną opcję");
                    break;
            }
        }
    }
    else
    {
        Console.WriteLine("Taka postać nie istnieje.");
    }
    }
}
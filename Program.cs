// static Character CreateCharacter()
// {
//     int age;
//     int character_class;
//     string character_class_name="";
//     Console.WriteLine("Podaj imię postaci: ");
//     string name = Console.ReadLine();
//     while(true)
//     {
//         Console.WriteLine("Podaj wiek postaci: ");
//         bool success = int.TryParse(Console.ReadLine(),out age);
//         if(success && age>0)
//         {
//             break;       
//         }
//         else
//         {
//             Console.WriteLine("Nieprawidłowy wiek.");
//         }
//     }
//     while(true)
//     {
//         Console.WriteLine("Wybierz klasę postaci:\n1.Wojownik\n2.Mag\n3.Łotrzyk");
//         bool classChoiceSuccess = int.TryParse(Console.ReadLine(), out character_class);
//         if(classChoiceSuccess && (character_class>0 && character_class<=3))
//         {
//             switch(character_class)
//             {
//                 case 1:
//                     character_class_name = "Wojownik";
//                     break;
//                 case 2: 
//                     character_class_name = "Mag";
//                     break;
//                 case 3:
//                     character_class_name = "Łotrzyk";
//                     break;
//             }
//             break;
//         }
//         else
//         {
//             Console.WriteLine("Wybierz poprawną klasę postaci.");
//         }
//     }
//     Character character = new Character();
//     character.Name = name;
//     character.Age = age;
//     character.ClassName = character_class_name;
//     return character;
// }
// Character character =CreateCharacter();
// Console.WriteLine($"\nTwoje imię to:{character.Name}\nMasz: {character.Age}\nTwoja klasa to: {character.ClassName}" );
Character geralt = new Character("Geralt", 14, CharacterClass.Warrior);
Character gandalf = new Character("Gandalf", 50, CharacterClass.Mage);
Character vancleef = new Character("vancleef", 32,CharacterClass.Rogue);
List<Character> characters = new();
characters.Add(geralt);
characters.Add(gandalf);
characters.Add(vancleef);
Character arthur = new Character("Arthur",30,CharacterClass.Warrior);
characters.Add(arthur);
bool running = true;
while(running)
{
    Console.WriteLine("===== CHARACTER MANAGER =====\n1. Wyświetl wszystkie postacie.\n2. Znajdź postać\n3. Wyświetl wojowników\n4. Statystyki\n5. Grupowanie po klasie\n6. Dodaj postać\n7. Usuń postać\n8. Edytuj postać\n0. Wyjście\nWybierz opcję:");
    int userChoice;
    bool success = int.TryParse(Console.ReadLine(), out userChoice);
    if(!success)
    {
        Console.WriteLine("Podaj liczbę.");
    }
    else
    {
        switch(userChoice)
        {
            case 1:
                ShowCharacters(characters);
                break;
            case 2:
                FindCharacter(characters);
                break;
            case 3:
                ShowWarriors(characters);
                break;
            case 4:
                ShowStatistics(characters);
                break;
            case 5:
                ShowGroups(characters);
                break;
            case 6:
                AddCharacter(characters);
                break;
            case 7:
                RemoveCharacter(characters);
                break;
            case 8:
                EditCharacter(characters);
                break;
            case 0:
                Console.WriteLine("Wybrano: Wyjście");
                running = false;
                break;
            default:
                Console.WriteLine("Nieprawidłowa opcja");
                break;
        }
    }

}

static void ShowCharacters(List<Character> characters)
{
    foreach(Character character in characters)
                {
                    Console.WriteLine($"{character.Name} - {character.ClassName} - {character.Age}");
                }
}

static void FindCharacter(List<Character> characters)
{
    Console.WriteLine("Podaj nazwę postaci:");
                string name = Console.ReadLine();
                Character? foundCharacter = characters.FirstOrDefault(
                    character => character.Name == name
                );
                if(foundCharacter != null)
                {
                    Console.WriteLine($"Znaleziono:\n{foundCharacter.Name} - {foundCharacter.ClassName} - {foundCharacter.Age}");
                }
                else
                {
                    Console.WriteLine("Taka postać nie istnieje.");
                }
}

static void ShowWarriors(List<Character> characters)
{
    var warriorCharacters = characters.Where(character => character.ClassName == CharacterClass.Warrior);
                foreach(Character character in warriorCharacters)
                {
                Console.WriteLine($"{character.Name} - {character.ClassName} - {character.Age}"); 
                }
}

static void ShowStatistics(List<Character> characters)
{
    int oldestCharacter = characters.Max(character => character.Age);
                int youngestCharacter = characters.Min(character => character.Age);
                double averageCharacterAge = characters.Average(character => character.Age);
                int sumHP = characters.Sum(character => character.Health);
                Console.WriteLine($"Najstarsza postać:{oldestCharacter}\nNajmłodsza postać:{youngestCharacter}\nŚredni wiek:{averageCharacterAge}\nŁączne HP:{sumHP}");
}

static void ShowGroups(List<Character> characters)
{
    var groupedCharacters = characters.GroupBy(character => character.ClassName);
                foreach(var group in groupedCharacters)
                {
                    int groupCount = group.Count();
                    Console.WriteLine($"{group.Key}-{groupCount}");
                }
}

static void AddCharacter(List<Character> characters)
{
    Console.WriteLine("Podaj imię postaci: ");
    string name = Console.ReadLine();

    int age;

    while(true)
    {
        Console.WriteLine("Podaj wiek postaci: ");
        bool success = int.TryParse(Console.ReadLine(),out age);
        if(success && age>0)
        {
            break;
        }
        Console.WriteLine("Podaj poprawny wiek");
    }
    CharacterClass className = CharacterClass.None;
    while(true)
    {
        Console.WriteLine("Wybierz klasę postaci:\n1. Wojownik\n2. Mag\n3. Łotrzyk");
        int classChoice;
        bool classChoiceSuccess = int.TryParse(Console.ReadLine(),out classChoice);
        
        if(classChoiceSuccess && classChoice >= 1 && classChoice <= 3)
        {
            className = GetCharacterClass(classChoice);
            break;
        }
    }
    Character character = new Character(name,age,className);
    characters.Add(character);
}

static void RemoveCharacter(List<Character> characters)
{
    Console.WriteLine("Podaj nazwę postaci: ");
    string name = Console.ReadLine();

    Character? foundCharacter = characters.FirstOrDefault(character => character.Name == name);

    if(foundCharacter == null)
    {
        Console.WriteLine("Taka postać nie istnieje.");
    }
    else
    {
        characters.Remove(foundCharacter);
        Console.WriteLine("Usunięto postać pomyślnie.");
    }
}

static void EditCharacter(List<Character> characters)
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

static CharacterClass GetCharacterClass(int choice)
{
    switch(choice)
    {
        case 1:
            return CharacterClass.Warrior;
        case 2:
            return CharacterClass.Mage;
        case 3:
            return CharacterClass.Rogue;
        default:
            return CharacterClass.None;
    }
}




// var groupedCharacters = characters.GroupBy(character => character.ClassName);

// foreach(var group in groupedCharacters)
// {
//     double avgAge = group.Average(character => character.Age);
//     Console.WriteLine($"{group.Key} - średni wiek:{avgAge}");
// }



// double averageWarriorAge = characters
//     .Where(character => character.ClassName == "Wojownik")
//     .Average(character => character.Age);
// Console.WriteLine(averageWarriorAge);


// foreach(var group in groupedCharacters)
// {
    
//     // int groupCount = group.Count();
//     // Console.WriteLine($"{group.Key} - {groupCount}");
//     // foreach(Character character in group)
//     // {
//     //     Console.WriteLine($"{character.Name}");
//     // }
// }

// int totalHealth = characters
// .Sum(character => character.Health);

// Console.WriteLine(totalHealth);


// int oldestCharacter = characters.Max(character => character.Age);
// int youngestCharacter = characters.Min(character => character.Age);
// double averageAge = characters.Average(character => character.Age);

// Console.WriteLine($"Oldest:{oldestCharacter} Youngest:{youngestCharacter} Average:{averageAge}");



// var sortedCharacters = characters
// .OrderBy(character => character.ClassName)
// .ThenBy(character => character.Age);

// foreach(Character character in sortedCharacters)
// {
//     Console.WriteLine($"{character.Name} - {character.ClassName} - {character.Age}");
// }



// Character? mageCharacter = characters.FirstOrDefault(
//     character => character.ClassName == "Mag"
// );

// if(mageCharacter != null)
// {
//     Console.WriteLine($"{mageCharacter.Name}");
// }
// else
// {
//     Console.WriteLine("Nie znaleziono Maga.");
// }

// if(characters.Any(character => character.ClassName == "Łotrzyk"))
// {
//     Console.WriteLine("Łotrzyk jest na liście.");
// }
// else
// {
//     Console.WriteLine("Łotrzyka nie ma na liście.");
// }

// if(characters.All(character => character.Age>=18))
// {
//     Console.WriteLine("Wszystkie postacie są pełnoletnie.");
// }
// else
// {
//     Console.WriteLine("Nie wszyskie postacie są pełnoletnie.");
// }

// int characterCount = characters.Count(
//     character => character.ClassName == "Mag"
// );
// Console.WriteLine(characterCount);

// var sortedCharacters = characters.OrderByDescending(
//     character => character.Age
// );

// foreach(Character character in sortedCharacters)
// {
//     Console.WriteLine($"{character.Name} - {character.Age}");
// }

// var result = characters
// .Where(character => character.ClassName == "Wojownik" && character.Age > 18)
// .OrderByDescending(character => character.Age);

// foreach(Character character in result)
// {
//     Console.WriteLine($"{character.Name}-{character.Age}");
// }

// Character? foundCharacter = characters.Find(character => character.Name == "Geralt");
// Console.WriteLine(foundCharacter.Name);

// var warriors = characters.Where(character => character.ClassName == "Wojownik");

// var charClasses = characters.Select(character => character.ClassName);

// foreach(string charClass in charClasses)
// {
//     Console.WriteLine(charClass);
// }


//characters.Clear();

// int numberOfCharacters = characters.Count;

// Console.WriteLine($"Na liście jest:{numberOfCharacters}");

// characters.Remove(gandalf);
// characters.RemoveAt(0);
// foreach(Character character in characters)
// {
//     Console.WriteLine($"{character.Name} - {character.ClassName}");
// }



// if(characters.Count == 0)
// {
//     Console.WriteLine("Lista jest pusta.");
// }
// else
// {
//     Console.WriteLine("Lista nie jest pusta.");
// }

// if(characters.Contains(vancleef))
// {
//     Console.WriteLine("Vancleef znajduje się na liście.");
// }
// else
// {
//     Console.WriteLine("Nie ma go na liście.");
//}


// int numberOfCharacters = characters.Count();
// Console.WriteLine(numberOfCharacters);

// Console.WriteLine($"{characters[1].Name},{characters[1].ClassName}");


// foreach(Character character in characters)
// {
//     string charName = character.Name;
//     string charClass = character.ClassName;
//     Console.WriteLine($"{charName} - {charClass}");
// }

// character.Attack(character2);
// Console.WriteLine(character2.Health);
// Console.WriteLine(character.Name);
// Console.WriteLine(character.Age);
// Console.WriteLine(character.ClassName);
// Console.WriteLine(character.Health);
// character.Attack();
// character.TakeDamage(60);
// character.Heal(30);
// Console.WriteLine(character.Health);
// character.Heal(50);
// Console.WriteLine(character.Health);
// Console.WriteLine($"\nTwoje imię to:{name}\nMasz: {age}\nTwoja klasa to: {character_class_name}" );
// Character character = new Character();
// character.Name = name;
// character.Age = age;
// character.ClassName = character_class_name;
// Console.WriteLine($"\nTwoje imię to:{character.Name}\nMasz: {character.Age}\nTwoja klasa to: {character.ClassName}" );

// Character character2 = new Character();
// character2.Name = "Gandalf";
// character2.Age = 14;
// character2.ClassName = "Mag";
// Console.WriteLine($"\nTwoje imię to:{character2.Name}\nMasz: {character2.Age}\nTwoja klasa to: {character2.ClassName}" );
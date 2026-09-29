
CharacterManager manager = new CharacterManager();
Character geralt = new Character("Geralt", 14, CharacterClass.Warrior);
Character gandalf = new Character("Gandalf", 50, CharacterClass.Mage);
Character vancleef = new Character("vancleef", 32,CharacterClass.Rogue);
Character arthur = new Character("Arthur",30,CharacterClass.Warrior);
manager.AddCharacter(geralt);
manager.AddCharacter(gandalf);
manager.AddCharacter(vancleef);
manager.AddCharacter(arthur);



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
                manager.ShowCharacters();
                break;
            case 2:
                manager.FindCharacter();
                break;
            case 3:
                manager.ShowWarriors();
                break;
            case 4:
                manager.ShowStatistics();
                break;
            case 5:
                manager.ShowGroups();
                break;
            case 6:
                AddCharacter(manager);
                break;
            case 7:
                manager.RemoveCharacter();
                break;
            case 8:
                manager.EditCharacter();
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




static void AddCharacter(CharacterManager manager)
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
    manager.AddCharacter(character);
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

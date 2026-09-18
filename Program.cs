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

Character geralt = new Character("Geralt", 14, "Wojownik");
Character gandalf = new Character("Gandalf", 50, "Mag");
Character vancleef = new Character("vancleef", 32,"Łotrzyk");

List<Character> characters = new();
characters.Add(geralt);
characters.Add(gandalf);
characters.Add(vancleef);

Character arthur = new Character("Arthur",30,"Wojownik");
characters.Add(arthur);

// Character? foundCharacter = characters.Find(character => character.Name == "Geralt");
// Console.WriteLine(foundCharacter.Name);

// var warriors = characters.Where(character => character.ClassName == "Wojownik");

var charClasses = characters.Select(character => character.ClassName);

foreach(string charClass in charClasses)
{
    Console.WriteLine(charClass);
}


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
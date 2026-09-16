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

Character character = new Character("Geralt", -14, "Wojownik");

Console.WriteLine(character.Name);
Console.WriteLine(character.Age);
Console.WriteLine(character.ClassName);

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
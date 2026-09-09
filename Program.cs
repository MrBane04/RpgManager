// See https://aka.ms/new-console-template for more information
Console.WriteLine("Podaj imię postaci: ");
string name = Console.ReadLine();
Console.WriteLine("Podaj wiek postaci: ");
try 
{
    bool canBeATeamMember = true;
    int age = int.Parse(Console.ReadLine());
    if(age <= 0){
        Console.WriteLine("Nieprawidłowy wiek");
    }
    else if(age <= 12)
    {
        canBeATeamMember = false;
        Console.WriteLine($"Twoja postać to {name}.\nKategoria:Dziecko\nMoże być członkiem drużyny {canBeATeamMember}");
    }
    else if(age >= 13 && age <= 17)
    {
        Console.WriteLine($"Twoja postać to {name}.\nKategoria:Nastolatek\nMoże być członkiem drużyny {canBeATeamMember}");
    }
    else if(age <= 60)
    {
        Console.WriteLine($"Twoja postać to {name}.\nKategoria:Dorosły\nMoże być członkiem drużyny {canBeATeamMember}");
    }
    else 
    {
        canBeATeamMember = false;
        Console.WriteLine($"Twoja postać to {name}.\nKategoria:Dorosły\nMoże być członkiem drużyny {canBeATeamMember}");
    }
}
catch(FormatException)
{
    Console.WriteLine("Nieprawidłowy wiek");
}
try
{
        Console.WriteLine("Wybierz klasę postaci:\n1.Wojownik\n2.Mag\n3.Łotrzyk");
int classChoice = int.Parse(Console.ReadLine());
switch(classChoice)
{
    case 1:
        Console.WriteLine("Wybrano Wojownika");
        break;
    case 2:
        Console.WriteLine("Wybrano Maga");
        break;
    case 3:
        Console.WriteLine("Wybrano Łotrzyka");
        break;
    default:
        Console.WriteLine("Nieprawidłowa klasa");
        break;
}
}
catch(FormatException)
{
    Console.WriteLine("Nieprawidłowa klasa");
}
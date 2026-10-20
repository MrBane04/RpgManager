using Spectre.Console;

bool isRunning = true;

while(isRunning)
{
    var choice = new SelectionPrompt<string>()
    .Title("RPG Manager - menu główne")
    .AddChoices("Moje kampanie", "Sesja RPG", "Wyjście");

    var selectedOption = AnsiConsole.Prompt(choice);

    AnsiConsole.MarkupLine($"Wybrano: [green]{selectedOption}[/]");

    switch (selectedOption)
    {
        case "Moje kampanie":
            AnsiConsole.MarkupLine("Wybrano kampanie");
            break;
        case "Sesja RPG":
            AnsiConsole.MarkupLine("Wybrano sesje");
            break;
        case "Wyjście":
            AnsiConsole.MarkupLine("Wybrano wyjście");
            isRunning = false;
            break;
        default:
            break;
    }
}

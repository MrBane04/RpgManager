using Spectre.Console;
using RpgManager.Core.Models;

bool isRunning = true;

List<Campaign> campaigns = new List<Campaign>();
Campaign glina = new Campaign("Glina","Glina","Aktywna");
Campaign zaczarowanyDab = new Campaign("Zaczarowany Dąb","DND","Wygasła");
campaigns.Add(glina);
campaigns.Add(zaczarowanyDab);
campaigns.Add(new Campaign ("Rozdroże Kruków", "Wiedźmin", "Aktywna"));
while (isRunning)
{
    var choice = new SelectionPrompt<string>()
    .Title("RPG Manager - menu główne")
    .AddChoices("Moje kampanie", "Sesja RPG", "Wyjście");

    var selectedOption = AnsiConsole.Prompt(choice);

    AnsiConsole.MarkupLine($"Wybrano: [green]{selectedOption}[/]");

    switch (selectedOption)
    {
        case "Moje kampanie":
            var table = new Table();

            table.AddColumn("Nazwa kampanii");
            table.AddColumn("System");
            table.AddColumn("Status");
            foreach(Campaign campaign in campaigns)
            {
                table.AddRow(campaign.Name, campaign.System, campaign.Status);
            }    
            

            AnsiConsole.Write(table);
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

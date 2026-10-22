using RpgManager.Core.Models;
using RpgManager.Core.Services;
using Spectre.Console;

namespace RpgManager.Console
{
    internal class ConsoleApp
    {
        private readonly CampaignService _campaignService;
        public ConsoleApp(CampaignService campaignService)
        {
            _campaignService = campaignService;
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                var choice = new SelectionPrompt<string>()
                .Title("RPG Manager - menu główne")
                .AddChoices("Dodaj kampanię", "Moje kampanie", "Sesja RPG", "Wyjście");

                var selectedOption = AnsiConsole.Prompt(choice);

                AnsiConsole.MarkupLine($"Wybrano: [green]{selectedOption}[/]");

                switch (selectedOption)
                {
                    case "Dodaj kampanię":
                        AddCampaign();
                        break;
                    case "Moje kampanie":
                        ShowCampaigns();
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
        }

        void AddCampaign()
        {
            var name = AnsiConsole.Ask<string>("Podaj nazwę kampanii");
            var system = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz system RPG")
                    .AddChoices("DND", "Jedyny Pierścień")
                );
            OperationResult creatingCampaign = _campaignService.CreateCampaign(name, system, "Aktywna");
            if (creatingCampaign.Success)
            {
                AnsiConsole.MarkupLine($"[green]{creatingCampaign.Message}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{creatingCampaign.Message}[/]");
            }

        }

        void ShowCampaigns()
        {
            var table = new Table();

            table.AddColumn("Nazwa kampanii");
            table.AddColumn("System");
            table.AddColumn("Status");
            foreach (Campaign campaign in _campaignService.GetCampaigns())
            {
                table.AddRow(campaign.Name, campaign.System, campaign.Status);
            }
            AnsiConsole.Write(table);
        }
    }
}

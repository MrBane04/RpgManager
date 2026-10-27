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
                AnsiConsole.Clear();
                var choice = new SelectionPrompt<string>()
                .Title("RPG Manager - menu główne")
                .AddChoices("Dodaj kampanię", "Moje kampanie","Sesja RPG", "Wyjście");

                var selectedOption = AnsiConsole.Prompt(choice);

                switch (selectedOption)
                {
                    case "Dodaj kampanię":
                        AddCampaign();
                        break;
                    case "Moje kampanie":
                        CampaignMenu();
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

        private void AddCampaign()
        {
            var name = AnsiConsole.Ask<string>("Podaj nazwę kampanii");
            var system = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz system RPG")
                    .AddChoices("DND", "Jedyny Pierścień")
                );
            OperationResult creatingCampaign = _campaignService.CreateCampaign(name, system, CampaignStatus.Aktywna);
            ShowResult(creatingCampaign);

        }

        private void CampaignMenu()
        {
            bool isRunningCM = true;

            while(isRunningCM)
            {
                AnsiConsole.Clear();
                var choiceCM = new SelectionPrompt<string>()
                .Title("Menu kampanii")
                .AddChoices("Wyświetl kampanie", "Edytuj kampanie", "Usuń kampanie", "Powrót");

                var selectedOptionCM = AnsiConsole.Prompt(choiceCM);

                switch (selectedOptionCM)
                {
                    case "Wyświetl kampanie":
                        ShowCampaigns();
                        WaitForContinue();
                        break;
                    case "Edytuj kampanie":
                        EditCampaign();
                        break;
                    case "Usuń kampanie":
                        DeleteCampaign();
                        break;
                    case "Powrót":
                        isRunningCM = false;
                        break;
                }
            }
        }

        private void DeleteCampaign()
        {
            ShowCampaigns();
            int id = AnsiConsole.Ask<int>("Podaj id kampanii do usunięcia: ");
            bool confirm = AnsiConsole.Confirm("Czy na pewno chcesz usunąć tę kampanię? ");
            if(confirm)
            {
                OperationResult result = _campaignService.DeleteCampaign(id);
                AnsiConsole.WriteLine(result.Message);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Anulowano usuwanie kampanii[/]");
            }
            WaitForContinue();
        }

        private void EditCampaign()
        {
            ShowCampaigns();
            int id = AnsiConsole.Ask<int>("Podaj id kampanii do edycji: ");
            string name = AnsiConsole.Ask<string>("Podaj nową nazwę kampanii: ");
            string system = AnsiConsole.Prompt(
                new SelectionPrompt<string>()
                    .Title("Wybierz system RPG")
                    .AddChoices("DND", "Jedyny Pierścień")
                );
            CampaignStatus status = AnsiConsole.Prompt(
                new SelectionPrompt<CampaignStatus>()
                    .Title("Wybierz status kampanii")
                    .AddChoices(CampaignStatus.Aktywna, CampaignStatus.Wygasła)
                );
            OperationResult result = _campaignService.EditCampaign(id, name, system, status);
            ShowResult(result);
            WaitForContinue();
        }

        private void ShowCampaigns()
        {
            IReadOnlyList<Campaign> campaignList = _campaignService.GetCampaigns();

            if(campaignList.Any())
            {
                var table = new Table()
                    .BorderColor(Color.Grey)
                    .Title("[bold yellow]Moje kampanie[/]");

                table.AddColumn("[yellow]Id[/]", col => col.Centered());
                table.AddColumn("[yellow]Nazwa kampanii[/]", col => col.Centered());
                table.AddColumn("[yellow]System[/]", col => col.Centered());
                table.AddColumn("[yellow]Status[/]", col => col.Centered());
                foreach (Campaign campaign in campaignList)
                {
                    table.AddRow(campaign.Id.ToString(),campaign.Name, campaign.System, campaign.Status.ToString());
                }
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Nie masz jeszcze żadnych kampanii[/]");
            }
        }

        private void WaitForContinue()
        {
            AnsiConsole.MarkupLine("[grey]Naciśnij dowolny klawisz, aby kontynuować...[/]");
            System.Console.ReadKey(true);
        }

        private void ShowResult(OperationResult result)
        {
            if (result.Success)
            {
                AnsiConsole.MarkupLine($"[green]{result.Message}[/]");
            }
            else
            {
                AnsiConsole.MarkupLine($"[red]{result.Message}[/]");
            }
        }
    }
}

using RpgManager.Core.Models;
using RpgManager.Core.Services;
using Spectre.Console;
using System.Net.NetworkInformation;
using System.Globalization;
using System.Runtime.CompilerServices;

namespace RpgManager.Console
{
    internal class ConsoleApp
    {
        private readonly CampaignService _campaignService;
        private readonly SessionService _sessionService;
        public ConsoleApp(CampaignService campaignService, SessionService sessionService)
        {
            _campaignService = campaignService;
            _sessionService = sessionService;
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
                        SessionMenu();
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

        private void SessionMenu()
        {
            IReadOnlyList<Campaign> campaignList = _campaignService.GetCampaigns();

            if(!campaignList.Any())
            {
                AnsiConsole.MarkupLine("[red]Lista kampanii jest pusta.[/]");
                WaitForContinue();
                return;
            }  
            var selectedCampaign = AnsiConsole.Prompt(  
                new SelectionPrompt<Campaign>() 
                .Title("Wybierz kampanię")
                .UseConverter(c => $"{c.Name} ({c.System})")   
                .AddChoices(campaignList));

            SessionOperationsMenu(selectedCampaign.Id);


        }

        private void SessionOperationsMenu(int campaignId)
        {
            bool isRunning = true;

            while (isRunning)
            {
                AnsiConsole.Clear();

                var choice = new SelectionPrompt<string>()
                    .Title("Menu sesji RPG")
                    .AddChoices(
                        "Wyświetl sesje",
                        "Szczegóły sesji",
                        "Dodaj sesję",
                        "Edytuj sesję",
                        "Usuń sesję",
                        "Powrót"
                    );

                var selectedOption = AnsiConsole.Prompt(choice);

                switch (selectedOption)
                {
                    case "Wyświetl sesje":
                        ShowSessions(campaignId);
                        break;
                    case "Szczegóły sesji":
                        SelectSessionForDetails(campaignId);
                        break;
                    case "Dodaj sesję":
                        AddSession(campaignId);
                        break;
                    case "Edytuj sesję":
                        EditSession(campaignId);
                        break;
                    case "Usuń sesję":
                        DeleteSession(campaignId);
                        break;
                    case "Powrót":
                        isRunning = false;
                        break;
                }
            }
        }

        private void ShowSessions(int campaignId)
        {
            IReadOnlyList<Session> sessionsList = _sessionService.GetSessions(campaignId);
            if(sessionsList.Any())
            {
                var table = new Table()
                    .BorderColor(Color.Grey)
                    .Title("[bold yellow]Sesje RPG[/]");

                table.AddColumn("[yellow]Id[/]", col => col.Centered());
                table.AddColumn("[yellow]Tytuł sesji[/]", col => col.Centered());
                table.AddColumn("[yellow]Data sesji[/]", col => col.Centered());
                table.AddColumn("[yellow]czas trwania(min)[/]", col => col.Centered());
                table.AddColumn("[yellow]Status(min)[/]", col => col.Centered());
                foreach (Session session in sessionsList)
                {
                    table.AddRow(
                        session.Id.ToString(),
                        session.Title,
                        session.SessionDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm"),
                        session.DurationMinutes?.ToString() ?? "Nie podano",
                        session.Status.ToString());
                }
                AnsiConsole.Write(table);
            }
            else
            {
                AnsiConsole.WriteLine("Nie ma żadnej sesji w tej kampanii.");
            }
            WaitForContinue();
        }

        private void SelectSessionForDetails(int campaignId)
        {
            IReadOnlyList<Session> sessionsList = _sessionService.GetSessions(campaignId);

            if (!sessionsList.Any())
            {
                AnsiConsole.MarkupLine("[red]Nie ma żadnych sesji do wyświetlenia.[/]");
                WaitForContinue();
                return;
            }

            var selectedSession = AnsiConsole.Prompt(
                new SelectionPrompt<Session>()
                    .Title("Wybierz sesję")
                    .UseConverter(s => $"{s.Title} ({s.SessionDate.ToLocalTime():dd.MM.yyyy HH:mm})")
                    .AddChoices(sessionsList)
            );

            ShowSessionDetails(campaignId, selectedSession.Id);
        }
        private void AddSession(int campaignId)
        {
            var title = AnsiConsole.Prompt(
                new TextPrompt<string>("Podaj nazwę sesji")
                .Validate(
                    input => !string.IsNullOrWhiteSpace(input),
                    "Podaj poprawną nazwę sesji !")
                );
            var description = AnsiConsole.Prompt(
                new TextPrompt<string>("Podaj opis sesji (opcjonalnie):")
                .AllowEmpty());

            DateTime sessionDate;

            while (true)
            {
                var dateInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Podaj datę i godzinę sesji (dd.MM.yyyy HH:mm):"));

                if (DateTime.TryParseExact(
                    dateInput,
                    "dd.MM.yyyy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out sessionDate))
                {
                    sessionDate = sessionDate.ToUniversalTime();
                    break;
                }

                AnsiConsole.MarkupLine("[red]Niepoprawny format daty![/]");

            }
            int? durationTime;

            while(true)
            {

                var durationInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Podaj czas trwania sesji w minutach (opcjonalne):").AllowEmpty());

                if(string.IsNullOrWhiteSpace(durationInput))
                {
                    durationTime = null;
                    break;
                }
                else if(int.TryParse(durationInput, out int parsedDuration) && parsedDuration>0)
                {
                    durationTime = parsedDuration;
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Podaj poprawną liczbę większą od zera![/]");
                }
            }

            SessionStatus status = AnsiConsole.Prompt(
                new SelectionPrompt<SessionStatus>()
                .Title("Wybierz status sesji")
                .AddChoices(
                    SessionStatus.Planned,
                    SessionStatus.Completed,
                    SessionStatus.Canceled));

            OperationResult creatingSession = _sessionService.CreateSession(campaignId,title,
                description,sessionDate,
                durationTime,status);
            ShowResult(creatingSession);
            WaitForContinue();
        }

        private void EditSession(int campaignId)
        {
            IReadOnlyList<Session> sessionsList = _sessionService.GetSessions(campaignId);

            if (!sessionsList.Any())
            {
                AnsiConsole.MarkupLine("[red]Nie ma żadnych sesji do edycji.[/]");
                WaitForContinue();
                return;
            }

            var selectedSession = AnsiConsole.Prompt(
                new SelectionPrompt<Session>()
                    .Title("Wybierz sesję do edycji")
                    .UseConverter(s => $"{s.Id} - {s.Title} ({s.SessionDate.ToLocalTime():dd.MM.yyyy HH:mm})")
                    .AddChoices(sessionsList)
            );

            var title = AnsiConsole.Prompt(
                new TextPrompt<string>("Podaj nowy tytuł sesji:")
                    .Validate(
                        input => !string.IsNullOrWhiteSpace(input),
                        "Tytuł sesji nie może być pusty!"
                    )
            );

            var description = AnsiConsole.Prompt(
                new TextPrompt<string>("Podaj nowy opis sesji (opcjonalnie):")
                    .AllowEmpty()
            );

            DateTime sessionDate;

            while (true)
            {
                var dateInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Podaj nową datę i godzinę (dd.MM.yyyy HH:mm):")
                );

                if (DateTime.TryParseExact(
                    dateInput,
                    "dd.MM.yyyy HH:mm",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.AssumeLocal,
                    out sessionDate))
                {
                    sessionDate = sessionDate.ToUniversalTime();
                    break;
                }

                AnsiConsole.MarkupLine("[red]Niepoprawny format daty![/]");
            }

            int? durationTime;

            while (true)
            {
                var durationInput = AnsiConsole.Prompt(
                    new TextPrompt<string>("Podaj nowy czas trwania w minutach (opcjonalnie):")
                        .AllowEmpty()
                );

                if (string.IsNullOrWhiteSpace(durationInput))
                {
                    durationTime = null;
                    break;
                }
                else if (int.TryParse(durationInput, out int parsedDuration) && parsedDuration > 0)
                {
                    durationTime = parsedDuration;
                    break;
                }
                else
                {
                    AnsiConsole.MarkupLine("[red]Podaj poprawną liczbę większą od zera![/]");
                }
            }

            SessionStatus status = AnsiConsole.Prompt(
                new SelectionPrompt<SessionStatus>()
                    .Title("Wybierz nowy status sesji")
                    .AddChoices(
                        SessionStatus.Planned,
                        SessionStatus.Completed,
                        SessionStatus.Canceled
                    )
            );

            bool confirm = AnsiConsole.Confirm(
                $"Czy na pewno chcesz zapisać zmiany w sesji [yellow]{selectedSession.Title}[/]?");

            if (confirm)
            {
                OperationResult result = _sessionService.EditSession(
                    campaignId,
                    selectedSession.Id,
                    durationTime,
                    title,
                    description,
                    sessionDate,
                    status
                );

                ShowResult(result);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Anulowano edycję sesji.[/]");
            }

            WaitForContinue();
        }

        private void DeleteSession(int campaignId)
        {
            IReadOnlyList<Session> sessionsList = _sessionService.GetSessions(campaignId);

            if (!sessionsList.Any())
            {
                AnsiConsole.MarkupLine("[red]Nie ma żadnych sesji do usunięcia.[/]");
                WaitForContinue();
                return;
            }

            var selectedSession = AnsiConsole.Prompt(
                new SelectionPrompt<Session>()
                    .Title("Wybierz sesję do usunięcia")
                    .UseConverter(s => $"{s.Id} - {s.Title} ({s.SessionDate.ToLocalTime():dd.MM.yyyy HH:mm})")
                    .AddChoices(sessionsList)
            );

            bool confirm = AnsiConsole.Confirm(
                $"Czy na pewno chcesz usunąć sesję [yellow]{selectedSession.Title}[/]?");

            if (confirm)
            {
                OperationResult result = _sessionService.DeleteSession(campaignId, selectedSession.Id);
                ShowResult(result);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Anulowano usuwanie sesji.[/]");
            }

            WaitForContinue();
        }

        private void ShowSessionDetails(int campaignId,int sessionId)
        {
            Session? session = _sessionService.GetSession(campaignId, sessionId);
            if (session != null)
            {
                AnsiConsole.MarkupLine("[bold yellow]Szczegóły sesji[/]");
                AnsiConsole.WriteLine();
                AnsiConsole.MarkupLine($"[yellow]ID:[/] {session.Id}");
                AnsiConsole.MarkupLine($"[yellow]Tytuł:[/] {Markup.Escape(session.Title)}");
                AnsiConsole.MarkupLine($"[yellow]Opis:[/] {Markup.Escape(session.Description ?? "Brak opisu")}");
                AnsiConsole.MarkupLine($"[yellow]Data:[/] {session.SessionDate:dd.MM.yyyy HH:mm}");
                AnsiConsole.MarkupLine($"[yellow]Czas trwania:[/] {session.DurationMinutes?.ToString() ?? "Nie podano"}");
                AnsiConsole.MarkupLine($"[yellow]Status:[/] {session.Status}");
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Nie znaleziono sesji w tej kampanii.[/]");
            }
            WaitForContinue();
        }

        private void AddCampaign()
        {
            var name = AnsiConsole.Prompt(
                new TextPrompt<string>("Podaj nazwę kampanii")
                .Validate(
                    input => !string.IsNullOrWhiteSpace(input),
                    "Podaj poprawną nazwę kampanii !")
                );
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
            bool hasCampaigns = ShowCampaigns();
            if(hasCampaigns)
            {
                int id=0;
                bool idFound = false;

                while(!idFound)
                {
                    id = AnsiConsole.Ask<int>("Podaj id kampanii do usunięcia: ");
                    idFound = _campaignService.CampaignExists(id);

                    if(!idFound)
                    {
                        AnsiConsole.MarkupLine("[red]Nie znaleziono kampanii o podanym id[/]");
                    }
                }
                bool confirm = AnsiConsole.Confirm("Czy na pewno chcesz usunąć tę kampanię? ");
                if (confirm)
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
        }

        private void EditCampaign()
        {
            bool hasCampaigns = ShowCampaigns();

            if(hasCampaigns)
            {
                int id =0;
                bool idFound = false;
                while(!idFound)
                {
                    id = AnsiConsole.Ask<int>("Podaj id kampanii do edycji: ");
                    idFound = _campaignService.CampaignExists(id);

                    if(!idFound)
                    {
                        AnsiConsole.MarkupLine("[red]Nie znaleziono kampanii o podanym id. Spróbuj ponownie.[/]");
                    }

                }
                var name = AnsiConsole.Prompt(
                    new TextPrompt<string>("Podaj nazwę kampanii")
                    .Validate(input => !string.IsNullOrWhiteSpace(input),
                    "Podaj poprawną nazwę kampanii ! "));
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

        }

        private bool ShowCampaigns()
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
                return true;
            }
            else
            {
                AnsiConsole.MarkupLine("[red]Nie masz jeszcze żadnych kampanii[/]");
                return false;
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

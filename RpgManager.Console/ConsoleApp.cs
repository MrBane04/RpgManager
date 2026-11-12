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
        private readonly CharacterService _characterService;
        private readonly NoteService _noteService;
        public ConsoleApp(CampaignService campaignService, SessionService sessionService, CharacterService characterService, NoteService noteService)
        {
            _campaignService = campaignService;
            _sessionService = sessionService;
            _characterService = characterService;
            _noteService = noteService;
        }

        public void Run()
        {
            bool isRunning = true;
            while (isRunning)
            {
                AnsiConsole.Clear();
                var choice = new SelectionPrompt<string>()
                .Title("RPG Manager - menu główne")
                .AddChoices("Dodaj kampanię", "Moje kampanie","Sesja RPG", "Postacie RPG", "Wyjście");

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
                    case "Postacie RPG":
                        CharacterMenu();
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

        public void CharacterMenu()
        {
            IReadOnlyList<Campaign> campaignList = _campaignService.GetCampaigns();

            if (!campaignList.Any())
            {
                AnsiConsole.MarkupLine("[red]Nie ma jeszcze żadnych kampanii.[/]");
                WaitForContinue();
                return;
            }
            var selectedCampaign = AnsiConsole.Prompt(
                new SelectionPrompt<Campaign>()
                .Title("Wybierz kampanię")
                .UseConverter(c => $"{c.Name} ({c.System})")
                .AddChoices(campaignList));

            CharacterOperationsMenu(selectedCampaign.Id);
        }

        public void CharacterOperationsMenu(int campaignId)
        {
            bool isRunning = true;

            while (isRunning)
            {
                AnsiConsole.Clear();

                var choice = AnsiConsole.Prompt(
                    new SelectionPrompt<string>()
                        .Title("Zarządzanie postaciami")
                        .AddChoices(
                            "Wyświetl postacie",
                            "Szczegóły postaci",
                            "Dodaj postać",
                            "Edytuj postać",
                            "Usuń postać",
                            "Powrót"));

                switch (choice)
                {
                    case "Wyświetl postacie":
                        ShowCharacters(campaignId);
                        break;
                    case "Szczegóły postaci":
                        SelectCharacterForDetails(campaignId);
                        break;
                    case "Dodaj postać":
                        AddCharacter(campaignId);
                        break;
                    case "Edytuj postać":
                        EditCharacter(campaignId);
                        break;
                    case "Usuń postać":
                        DeleteCharacter(campaignId);
                        break;
                    case "Powrót":
                        isRunning = false;
                        break;
                }
            }
        }

        public void ShowCharacters(int campaignId)
        {
            int page = 1;
            int pageSize = 10;
            int totalCharacters = _characterService.CountCharacters(campaignId);
            int totalPages = (int)Math.Ceiling((double)totalCharacters / pageSize);
            if(totalCharacters ==0)
            {
                AnsiConsole.MarkupLine("[yellow]Ta kampania nie ma żadnych postaci.[/]");
                WaitForContinue();
                return;
            }

            bool isRunning = true;
            while(isRunning)
            {
                AnsiConsole.Clear();
                IReadOnlyList<Character> characters = _characterService.GetCharacters(campaignId, page,pageSize);
                var table = new Table();
                table.AddColumn("ID");
                table.AddColumn("Imię");
                table.AddColumn("Wiek");
                table.AddColumn("Rasa");
                table.AddColumn("Klasa");
                table.AddColumn("Poziom");

                foreach (var character in characters)
                {
                    table.AddRow(
                        character.Id.ToString(),
                        character.Name,
                        character.Age.ToString(),
                        character.Race ?? "-",
                        character.CharacterClass ?? "-",
                        character.Level.ToString()
                    );
                }
                AnsiConsole.Write(table);
                AnsiConsole.MarkupLine($"[grey]Strona {page} z {totalPages}[/]");
                AnsiConsole.MarkupLine("[green]N[/] - Następna strona | [green]P[/] - Poprzednia strona | [green]Q[/] - Powrót");
                ConsoleKeyInfo keyInfo = System.Console.ReadKey(true);
                switch(keyInfo.Key)
                {
                    case ConsoleKey.N:
                        if(page < totalPages)
                        { page++; }
                        break;
                    case ConsoleKey.P:
                        if(page>1)
                        { page--; }
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        isRunning = false;
                        break;
                }
            }


            
        }

        public void SelectCharacterForDetails(int campaignId)
        {
            IReadOnlyList<Character> characters = _characterService.GetCharacters(campaignId);

            if (!characters.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Brak postaci do wyświetlenia.[/]");
                WaitForContinue();
                return;
            }

            var selectedCharacter = AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title("Wybierz postać")
                    .UseConverter(c => $"{c.Name} (ID: {c.Id})")
                    .AddChoices(characters));

            ShowCharacterDetails(campaignId, selectedCharacter.Id);
        }

        public void ShowCharacterDetails(int campaignId, int characterId)
        {
            Character? character = _characterService.GetCharacter(campaignId, characterId);

            if (character == null)
            {
                AnsiConsole.MarkupLine("[red]Nie znaleziono postaci.[/]");
                WaitForContinue();
                return;
            }

            var panel = new Panel(
                $"[bold]Imię:[/] {Markup.Escape(character.Name)}\n" +
                $"[bold]Wiek:[/] {character.Age}\n" +
                $"[bold]Rasa:[/] {Markup.Escape(character.Race ?? "-")}\n" +
                $"[bold]Klasa:[/] {Markup.Escape(character.CharacterClass ?? "-")}\n" +
                $"[bold]Poziom:[/] {character.Level}\n" +
                $"[bold]Opis:[/] {Markup.Escape(character.Description ?? "Brak opisu")}\n" +
                $"[bold]Utworzono:[/] {character.CreatedAt.ToLocalTime():dd.MM.yyyy HH:mm}"
            );

            panel.Header = new PanelHeader($"Postać: {character.Name}");
            panel.Border = BoxBorder.Rounded;

            AnsiConsole.Write(panel);
            WaitForContinue();
        }

        public void AddCharacter(int campaignId)
        {
            AnsiConsole.MarkupLine("[bold]Dodawanie nowej postaci[/]");

            string name = AnsiConsole.Ask<string>("Imię postaci:");
            int age = AskPositiveNumber("Wiek:");
            string? race = AnsiConsole.Ask<string>("Rasa (opcjonalnie):");
            string? characterClass = AnsiConsole.Ask<string>("Klasa (opcjonalnie):");
            int level = AskPositiveNumber("Poziom:");
            string? description = AnsiConsole.Ask<string>("Opis (opcjonalnie):");

            race = string.IsNullOrWhiteSpace(race) ? null : race;
            characterClass = string.IsNullOrWhiteSpace(characterClass) ? null : characterClass;
            description = string.IsNullOrWhiteSpace(description) ? null : description;

            OperationResult result = _characterService.CreateCharacter(
                campaignId, name, age, race, characterClass, level, description);

            ShowResult(result);
            WaitForContinue();
        }

        public int AskPositiveNumber(string prompt)
        {
            int value;
            while(true)
            {
                value = AnsiConsole.Ask<int>(prompt);
                if(value >0)
                {
                    return value;
                }
                AnsiConsole.MarkupLine("[red]Wartość musi być większa od zera.[/]");
            }
        }

        public void EditCharacter(int campaignId)
        {
            IReadOnlyList<Character> characters = _characterService.GetCharacters(campaignId);

            if (!characters.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Brak postaci do edycji.[/]");
                WaitForContinue();
                return;
            }

            var selectedCharacter = AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title("Wybierz postać do edycji")
                    .UseConverter(c => $"{c.Name} (ID: {c.Id})")
                    .AddChoices(characters));

            string name = AnsiConsole.Prompt(
                new TextPrompt<string>("Nowe imię:")
                    .DefaultValue(selectedCharacter.Name));

            int age = AnsiConsole.Prompt(
                new TextPrompt<int>("Nowy wiek:")
                    .DefaultValue(selectedCharacter.Age));

            string? race = AnsiConsole.Prompt(
                new TextPrompt<string>("Nowa rasa:")
                    .DefaultValue(selectedCharacter.Race ?? ""));

            string? characterClass = AnsiConsole.Prompt(
                new TextPrompt<string>("Nowa klasa:")
                    .DefaultValue(selectedCharacter.CharacterClass ?? ""));

            int level = AnsiConsole.Prompt(
                new TextPrompt<int>("Nowy poziom:")
                    .DefaultValue(selectedCharacter.Level));

            string? description = AnsiConsole.Prompt(
                new TextPrompt<string>("Nowy opis:")
                    .DefaultValue(selectedCharacter.Description ?? ""));

            race = string.IsNullOrWhiteSpace(race) ? null : race;
            characterClass = string.IsNullOrWhiteSpace(characterClass) ? null : characterClass;
            description = string.IsNullOrWhiteSpace(description) ? null : description;

            bool confirm = AnsiConsole.Confirm(
    $"Czy na pewno chcesz zapisać zmiany w postaci [yellow]{Markup.Escape(selectedCharacter.Name)}[/]?");

            if (confirm)
            {
                OperationResult result = _characterService.EditCharacter(
                    campaignId,selectedCharacter.Id,
                    name,age,
                    race,characterClass,
                    level,description);

                ShowResult(result);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Anulowano edycję postaci.[/]");
            }

            WaitForContinue();
        }

        public void DeleteCharacter(int campaignId)
        {
            IReadOnlyList<Character> characters = _characterService.GetCharacters(campaignId);

            if (!characters.Any())
            {
                AnsiConsole.MarkupLine("[yellow]Brak postaci do usunięcia.[/]");
                WaitForContinue();
                return;
            }

            var selectedCharacter = AnsiConsole.Prompt(
                new SelectionPrompt<Character>()
                    .Title("Wybierz postać do usunięcia")
                    .UseConverter(c => $"{c.Name} (ID: {c.Id})")
                    .AddChoices(characters));

            bool confirmation = AnsiConsole.Confirm(
                $"Czy na pewno chcesz usunąć postać [red]{Markup.Escape(selectedCharacter.Name)}[/]?");

            if (!confirmation)
            {
                AnsiConsole.MarkupLine("[yellow]Anulowano usuwanie.[/]");
                WaitForContinue();
                return;
            }

            OperationResult result = _characterService.DeleteCharacter(
                campaignId, selectedCharacter.Id);

            ShowResult(result);
            WaitForContinue();
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
            int page = 1;
            int pageSize = 10;
            int totalSessions = _sessionService.CountSessions(campaignId);
            int totalPages = (int)Math.Ceiling((double)totalSessions / pageSize);
            if (totalSessions == 0)
            {
                AnsiConsole.MarkupLine("[yellow]Nie ma żadnej sesji w tej kampani.[/]");
                WaitForContinue();
                return;
            }
            bool isRunning = true;
            while (isRunning)
            {
                AnsiConsole.Clear();
                IReadOnlyList<Session> sessionsList = _sessionService.GetSessions(campaignId,page,pageSize);
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
                AnsiConsole.MarkupLine($"[grey]Strona {page}/{totalPages}[/]");
                AnsiConsole.MarkupLine("[green]N[/] - Następna strona | [green]P[/] - Poprzednia strona | [green]Q[/] - Powrót");
                ConsoleKeyInfo keyInfo = System.Console.ReadKey(true);
                //AnsiConsole.MarkupLine($"[red]Odczytany klawisz: {keyInfo.Key}[/]");
                switch (keyInfo.Key)
                {
                    case ConsoleKey.N:
                        if(page<totalPages)
                        {
                            page++;
                        }
                        break;
                    case ConsoleKey.P:
                        if(page >1)
                        {
                            page--;
                        }
                        break;
                    case ConsoleKey.Q:
                    case ConsoleKey.Escape:
                        isRunning = false;
                        break;
                }

            }
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
                .AddChoices("Wyświetl kampanie", "Edytuj kampanie", "Usuń kampanie","Notatka kampanii", "Powrót");

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
                    case "Notatka kampanii":
                        SelectCampaignForNote();
                        break;
                    case "Powrót":
                        isRunningCM = false;
                        break;
                }
            }
        }

        private void SelectCampaignForNote()
        {
            IReadOnlyList<Campaign> campaignList = _campaignService.GetCampaigns();

            if (!campaignList.Any())
            {
                AnsiConsole.MarkupLine("[red]Nie ma jeszcze żadnych kampanii.[/]");
                WaitForContinue();
                return;
            }

            var selectedCampaign = AnsiConsole.Prompt(
                new SelectionPrompt<Campaign>()
                    .Title("Wybierz kampanię, której notatkę chcesz otworzyć")
                    .UseConverter(c => $"{c.Name} ({c.System})")
                    .AddChoices(campaignList));

            OpenCampaignNote(selectedCampaign.Id);
        }

        private void OpenCampaignNote(int campaignId)
        {
            Note? note = _noteService.GetNote(campaignId);

            if (note == null)
            {
                AnsiConsole.MarkupLine("[red]Nie znaleziono notatki dla tej kampanii.[/]");
                WaitForContinue();
                return;
            }

            AnsiConsole.Clear();

            AnsiConsole.MarkupLine("[bold yellow]Notatka kampanii[/]");
            AnsiConsole.MarkupLine($"[grey]Ostatnia edycja: {note.UpdatedAt.ToLocalTime():dd.MM.yyyy HH:mm}[/]");
            AnsiConsole.WriteLine();

            NoteEditor editor = new NoteEditor(note.Content);
            string? updatedContent = editor.Edit();
            System.Console.WriteLine();
            if(updatedContent != null )
            {
                OperationResult result = _noteService.UpdateNote(campaignId, updatedContent);
                ShowResult(result);
            }
            else
            {
                AnsiConsole.MarkupLine("[yellow]Anulowano edycję notatki.[/]");
            }
            
            WaitForContinue();
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

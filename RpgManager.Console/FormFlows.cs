using System.Globalization;
using RpgManager.Core.Models;
using RpgManager.Core.Services;
using Terminal.Gui.App;
using Terminal.Gui.Views;

namespace RpgManager.Console;

internal sealed partial class ManagerWindow
{
    private void AddCampaign()
    {
        var form = new InputDialog("Nowa kampania", "Dodaj nowy świat do swojego panelu.",
            [new("Nazwa kampanii", ""), new("System", "D&D 5e")],
            values => string.IsNullOrWhiteSpace(values[0]) ? "Podaj nazwę kampanii." :
                string.IsNullOrWhiteSpace(values[1]) ? "Podaj nazwę systemu." : null);
        App!.Run(form);
        if (!form.Saved) return;

        ShowResult(_campaignService.CreateCampaign(form.Values[0], form.Values[1], CampaignStatus.Aktywna));
        Navigate(_section);
    }

    private void EditCampaign(Campaign campaign)
    {
        var form = new InputDialog("Edycja kampanii", "Zmień podstawowe informacje kampanii.",
            [new("Nazwa kampanii", campaign.Name), new("System", campaign.System), new("Status Aktywna/Wygasła", campaign.Status.ToString())],
            values => string.IsNullOrWhiteSpace(values[0]) ? "Podaj nazwę kampanii." :
                string.IsNullOrWhiteSpace(values[1]) ? "Podaj nazwę systemu." :
                Enum.TryParse(values[2], true, out CampaignStatus status) && Enum.IsDefined(status) ? null : "Status musi mieć wartość Aktywna albo Wygasła.");
        App!.Run(form);
        if (!form.Saved) return;

        Enum.TryParse(form.Values[2], true, out CampaignStatus parsedStatus);
        ShowResult(_campaignService.EditCampaign(campaign.Id, form.Values[0], form.Values[1], parsedStatus));
        Navigate("Kampanie");
    }

    private void DeleteCampaign(Campaign campaign)
    {
        if (!Confirm("Usuń kampanię", $"Usunąć kampanię „{campaign.Name}”?", "Usuń", "Anuluj"))
            return;

        ShowResult(_campaignService.DeleteCampaign(campaign.Id));
        if (_activeCampaignId == campaign.Id) _activeCampaignId = null;
        Navigate("Kampanie");
    }

    private void AddSession(Campaign campaign)
    {
        var form = new InputDialog("Nowa sesja", $"Dodaj termin do kampanii: {campaign.Name}.",
            [
                new("Tytuł sesji", ""),
                new("Opis", "", true),
                new("Data dd.MM.rrrr GG:mm", DateTime.Now.ToString("dd.MM.yyyy HH:mm")),
                new("Czas w minutach", ""),
                new("Status", UiKit.SessionStatusLabel(SessionStatus.Planned))
            ], ValidateSessionForm);
        App!.Run(form);
        if (!form.Saved) return;

        ParseDuration(form.Values[3], out var duration);
        ParseSessionStatus(form.Values[4], out var status);
        _ = ParseSessionDate(form.Values[2], out var date);
        ShowResult(_sessionService.CreateSession(campaign.Id, form.Values[0], NullIfEmpty(form.Values[1]), date.ToUniversalTime(), duration, status));
        Navigate("Sesje");
    }

    private void EditSession(Campaign campaign, Session session)
    {
        var form = new InputDialog("Edycja sesji", $"Zmień termin w kampanii: {campaign.Name}.",
            [
                new("Tytuł sesji", session.Title),
                new("Opis", session.Description ?? "", true),
                new("Data dd.MM.rrrr GG:mm", session.SessionDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm")),
                new("Czas w minutach", session.DurationMinutes?.ToString() ?? ""),
                new("Status", UiKit.SessionStatusLabel(session.Status))
            ], ValidateSessionForm);
        App!.Run(form);
        if (!form.Saved) return;

        ParseDuration(form.Values[3], out var duration);
        ParseSessionStatus(form.Values[4], out var status);
        _ = ParseSessionDate(form.Values[2], out var date);
        ShowResult(_sessionService.EditSession(campaign.Id, session.Id, duration, form.Values[0], NullIfEmpty(form.Values[1]), date.ToUniversalTime(), status));
        Navigate("Sesje");
    }

    private void DeleteSession(Campaign campaign, Session session)
    {
        if (!Confirm("Usuń sesję", $"Usunąć sesję „{session.Title}”?", "Usuń", "Anuluj"))
            return;

        ShowResult(_sessionService.DeleteSession(campaign.Id, session.Id));
        Navigate("Sesje");
    }

    private void AddCharacter(Campaign campaign)
    {
        var form = new InputDialog("Nowa postać", $"Dodaj bohatera do kampanii: {campaign.Name}.",
            [
                new("Imię postaci", ""),
                new("Wiek", "18"),
                new("Rasa", ""),
                new("Klasa", ""),
                new("Poziom", "1"),
                new("Opis", "", true)
            ], ValidateCharacterForm);
        App!.Run(form);
        if (!form.Saved) return;

        int.TryParse(form.Values[1], out var age);
        int.TryParse(form.Values[4], out var level);
        ShowResult(_characterService.CreateCharacter(campaign.Id, form.Values[0], age,
            NullIfEmpty(form.Values[2]), NullIfEmpty(form.Values[3]), level, NullIfEmpty(form.Values[5])));
        Navigate("Postacie");
    }

    private void EditCharacter(Campaign campaign, Character character)
    {
        var form = new InputDialog("Edycja postaci", $"Edytuj kartę bohatera: {character.Name}.",
            [
                new("Imię postaci", character.Name),
                new("Wiek", character.Age.ToString()),
                new("Rasa", character.Race ?? ""),
                new("Klasa", character.CharacterClass ?? ""),
                new("Poziom", character.Level.ToString()),
                new("Opis", character.Description ?? "", true)
            ], ValidateCharacterForm);
        App!.Run(form);
        if (!form.Saved) return;

        int.TryParse(form.Values[1], out var age);
        int.TryParse(form.Values[4], out var level);
        ShowResult(_characterService.EditCharacter(campaign.Id, character.Id, form.Values[0], age,
            NullIfEmpty(form.Values[2]), NullIfEmpty(form.Values[3]), level, NullIfEmpty(form.Values[5])));
        Navigate("Postacie");
    }

    private void DeleteCharacter(Campaign campaign, Character character)
    {
        if (!Confirm("Usuń postać", $"Usunąć postać „{character.Name}”?", "Usuń", "Anuluj"))
            return;

        ShowResult(_characterService.DeleteCharacter(campaign.Id, character.Id));
        Navigate("Postacie");
    }

    private static string? ValidateSessionForm(string[] values)
    {
        if (string.IsNullOrWhiteSpace(values[0])) return "Tytuł sesji jest wymagany.";
        if (!ParseSessionDate(values[2], out _)) return "Podaj datę w formacie dd.MM.rrrr GG:mm.";
        if (!string.IsNullOrWhiteSpace(values[3]) && (!int.TryParse(values[3], out var duration) || duration <= 0))
            return "Czas musi być pusty albo liczbą większą od zera.";
        if (!ParseSessionStatus(values[4], out _)) return "Wybierz Zaplanowana, Zakończona albo Odwołana.";
        return null;
    }

    private static string? ValidateCharacterForm(string[] values)
    {
        if (string.IsNullOrWhiteSpace(values[0])) return "Imię postaci jest wymagane.";
        if (!int.TryParse(values[1], out var age) || age <= 0) return "Wiek musi być liczbą większą od zera.";
        if (!int.TryParse(values[4], out var level) || level <= 0) return "Poziom musi być liczbą większą od zera.";
        return null;
    }

    private static bool ParseSessionDate(string input, out DateTime date) =>
        DateTime.TryParseExact(input, "dd.MM.yyyy HH:mm", CultureInfo.GetCultureInfo("pl-PL"), DateTimeStyles.AssumeLocal, out date);

    private static bool ParseSessionStatus(string input, out SessionStatus status)
    {
        var normalized = input.Trim();
        if (normalized.Equals("Zaplanowana", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Planned", StringComparison.OrdinalIgnoreCase))
            status = SessionStatus.Planned;
        else if (normalized.Equals("Zakończona", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Completed", StringComparison.OrdinalIgnoreCase))
            status = SessionStatus.Completed;
        else if (normalized.Equals("Odwołana", StringComparison.OrdinalIgnoreCase) || normalized.Equals("Canceled", StringComparison.OrdinalIgnoreCase))
            status = SessionStatus.Canceled;
        else
        {
            status = default;
            return false;
        }
        return true;
    }

    private static void ParseDuration(string input, out int? duration) =>
        duration = int.TryParse(input, out var parsed) ? parsed : null;

    private static string? NullIfEmpty(string value) => string.IsNullOrWhiteSpace(value) ? null : value;

    private void ShowResult(OperationResult result)
    {
        if (result.Success) ShowMessage("Zapisano", result.Message);
        else ShowError(result.Message);
    }

}

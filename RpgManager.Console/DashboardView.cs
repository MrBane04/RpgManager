using RpgManager.Core.Models;
using Terminal.Gui.App;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace RpgManager.Console;

internal sealed partial class ManagerWindow
{
    private void BuildDashboard()
    {
        var campaigns = _campaignService.GetCampaigns();
        var sessionCount = campaigns.Sum(c => _sessionService.CountSessions(c.Id));
        var characterCount = campaigns.Sum(c => _characterService.CountCharacters(c.Id));
        var activeCount = campaigns.Count(c => c.Status == CampaignStatus.Aktywna);

        _workspace.Add(
            UiKit.Metric("Kampanie", campaigns.Count.ToString(), 0, 0, Dim.Percent(25)),
            UiKit.Metric("Aktywne", activeCount.ToString(), Pos.Percent(25), 0, Dim.Percent(25)),
            UiKit.Metric("Sesje", sessionCount.ToString(), Pos.Percent(50), 0, Dim.Percent(25)),
            UiKit.Metric("Postacie", characterCount.ToString(), Pos.Percent(75), 0, Dim.Fill()));

        var campaignPanel = UiKit.Panel("TWOJE KAMPANIE", 0, 4, Dim.Percent(55), Dim.Fill());
        var contextPanel = UiKit.Panel("KAMPANIA W SKRÓCIE", Pos.Right(campaignPanel), 4, Dim.Fill(), Dim.Fill(), "RpgAccent");
        _workspace.Add(campaignPanel, contextPanel);

        var lines = campaigns.Select(c => UiKit.CampaignLine(
            c.Name, c.System, c.Status.ToString(),
            _sessionService.CountSessions(c.Id), _characterService.CountCharacters(c.Id))).ToList();
        if (lines.Count == 0)
            lines.Add("+ Utwórz pierwszą kampanię");

        campaignPanel.Add(UiKit.Label("KAMPANIA   SYSTEM  ST  S/P", 1, 0, Dim.Fill(2)));
        var list = UiKit.List(lines, 1, 1, Dim.Fill(2), Dim.Fill(2));
        if (campaigns.Count > 0)
            list.SelectedItem = 0;
        campaignPanel.Add(list);
        void RenderContext(int? selected)
        {
            contextPanel.RemoveAll();
            if (selected is not int index || index < 0 || index >= campaigns.Count)
            {
                contextPanel.Title = "KAMPANIA W SKRÓCIE";
                contextPanel.Add(UiKit.Label("Wybierz kampanię z listy, aby zobaczyć jej skrót.", 1, 1, Dim.Fill(2), 2));
                var actions = AddContextShortcuts(contextPanel, 4);
                SetFocusOrder(_navigation, list, actions);
                return;
            }

            var campaign = campaigns[index];
            _activeCampaignId = campaign.Id;
            _breadcrumb.Text = $"Pulpit   /   {campaign.Name}";
            var sessions = _sessionService.GetSessions(campaign.Id);
            var next = sessions.Where(s => s.Status == SessionStatus.Planned && s.SessionDate >= DateTime.UtcNow)
                .OrderBy(s => s.SessionDate).FirstOrDefault();

            contextPanel.Title = UiKit.Clip(campaign.Name, 20);
            contextPanel.Add(UiKit.Label($"{UiKit.Clip(campaign.System, 11)}  /  {campaign.Status}", 1, 1, Dim.Fill(2)));
            contextPanel.Add(UiKit.Label($"Sesje {_sessionService.CountSessions(campaign.Id)}   Postacie {_characterService.CountCharacters(campaign.Id)}", 1, 2, Dim.Fill(2)));

            var nextSessionLine = next is null
                ? "Brak terminu  /  dodaj w Sesjach"
                : $"{next.SessionDate.ToLocalTime():dd.MM HH:mm}  {UiKit.Clip(next.Title, 10)}";
            contextPanel.Add(UiKit.Label("NAJBLIŻSZA SESJA", 1, 4, Dim.Fill(2)));
            contextPanel.Add(UiKit.Label(nextSessionLine, 1, 5, Dim.Fill(2)));
            var shortcuts = AddContextShortcuts(contextPanel, 6);
            SetFocusOrder(_navigation, list, shortcuts);
        }

        list.ValueChanged += (_, e) => RenderContext(e.NewValue);
        list.Accepted += (_, _) =>
        {
            if (campaigns.Count == 0)
            {
                Navigate("Kampanie");
                return;
            }

            if (list.SelectedItem is int index && index >= 0 && index < campaigns.Count)
            {
                _activeCampaignId = campaigns[index].Id;
                Navigate("Kampanie");
            }
        };
        RenderContext(list.SelectedItem);
    }

    private ListView AddContextShortcuts(FrameView context, int y)
    {
        context.Add(UiKit.Label("PRZEJDŹ DO", 1, y, Dim.Fill(2)));
        var actions = UiKit.List(["Sesje", "Postacie", "Kronika"], 1, y + 1, Dim.Fill(2), 3);
        actions.SelectedItem = 0;
        actions.Accepted += (_, _) =>
        {
            if (actions.SelectedItem is not int index) return;
            Navigate(index switch { 0 => "Sesje", 1 => "Postacie", _ => "Kronika" });
        };
        context.Add(actions);
        return actions;
    }
}

using RpgManager.Core.Models;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace RpgManager.Console;

internal sealed partial class ManagerWindow
{
    private void BuildCampaigns()
    {
        var campaigns = _campaignService.GetCampaigns();
        var listPane = UiKit.Panel("LISTA KAMPANII", 0, 0, Dim.Percent(45), Dim.Fill());
        var detailPane = UiKit.Panel("SZCZEGÓŁY KAMPANII", Pos.Right(listPane), 0, Dim.Fill(), Dim.Fill(), "RpgAccent");
        _workspace.Add(listPane, detailPane);

        var entries = campaigns.Select(c =>
            $"{UiKit.Clip(c.Name, 12),-12} {ShortStatus(c.Status)} {_sessionService.CountSessions(c.Id)}/{_characterService.CountCharacters(c.Id)}").ToList();
        entries.Add("+  Nowa kampania");

        listPane.Add(UiKit.Label("KAMPANIA   ST  S/P", 1, 0, Dim.Fill(2)));
        var list = UiKit.List(entries, 1, 1, Dim.Fill(2), Dim.Fill());
        list.SelectedItem = 0;
        listPane.Add(list);

        void RenderDetails(int? selected)
        {
            detailPane.RemoveAll();
            if (selected is not int index || index < 0 || index >= campaigns.Count)
            {
                detailPane.Title = "SZCZEGÓŁY KAMPANII";
                if (campaigns.Count == 0)
                    detailPane.Add(UiKit.Label("Lista jest pusta. Wybierz „Nowa kampania”, aby utworzyć pierwszy wpis.", 2, 1, Dim.Fill(4), 3));
                else
                    detailPane.Add(UiKit.Label("Wybierz kampanię z listy. Liczniki i działania dotyczą wskazanego świata.", 2, 1, Dim.Fill(4), 3));
                SetFocusOrder(_navigation, list);
                return;
            }

            var campaign = campaigns[index];
            _activeCampaignId = campaign.Id;
            _breadcrumb.Text = $"Kampanie   /   {campaign.Name}";
            detailPane.Title = campaign.Name;

            var sessionCount = _sessionService.CountSessions(campaign.Id);
            var characterCount = _characterService.CountCharacters(campaign.Id);
            var note = _noteService.GetNote(campaign.Id);
            detailPane.Add(UiKit.Label(UiKit.DetailLine("System", campaign.System, 27), 1, 1, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Status", campaign.Status.ToString(), 27), 1, 2, Dim.Fill(2)));
            var nextSession = _sessionService.GetSessions(campaign.Id)
                .Where(s => s.Status == SessionStatus.Planned && s.SessionDate >= DateTime.UtcNow)
                .OrderBy(s => s.SessionDate).FirstOrDefault();
            detailPane.Add(UiKit.Label("ZAWARTOŚĆ", 1, 3, Dim.Fill(2)));
            detailPane.Add(UiKit.Label($"Sesje {sessionCount}", 1, 4, Dim.Percent(45)));
            detailPane.Add(UiKit.Label($"Postacie {characterCount}", Pos.Percent(48), 4, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(note is null ? "Kronika: brak wpisu" : $"Kronika: {note.UpdatedAt.ToLocalTime():dd.MM.yyyy}", 1, 5, Dim.Fill(2)));
            detailPane.Add(UiKit.Label("NAJBLIŻSZA SESJA", 1, 6, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(nextSession is null ? "Brak zaplanowanego terminu" :
                $"{nextSession.SessionDate.ToLocalTime():dd.MM HH:mm}  {UiKit.Clip(nextSession.Title, 13)}", 1, 7, Dim.Fill(2)));
            var actionPanel = UiKit.Panel("OPERACJE  /  ENTER", 1, 8, Dim.Fill(3), 8, "RpgAccent");
            var actions = UiKit.List(["Edytuj kampanię", "Usuń kampanię", "Sesje", "Postacie", "Kronika"],
                0, 0, Dim.Fill(), Dim.Fill());
            actions.SelectedItem = 0;
            actions.Accepted += (_, _) =>
            {
                switch (actions.SelectedItem)
                {
                    case 0: EditCampaign(campaign); break;
                    case 1: DeleteCampaign(campaign); break;
                    case 2: Navigate("Sesje"); break;
                    case 3: Navigate("Postacie"); break;
                    case 4: Navigate("Kronika"); break;
                }
            };
            actionPanel.Add(actions);
            detailPane.Add(actionPanel);
            SetFocusOrder(_navigation, list, actions);
        }

        list.ValueChanged += (_, args) => RenderDetails(args.NewValue);
        list.Accepted += (_, _) =>
        {
            if (list.SelectedItem == campaigns.Count)
                AddCampaign();
            else
                RenderDetails(list.SelectedItem);
        };
        RenderDetails(list.SelectedItem);
    }

    private static string ShortStatus(CampaignStatus status) => status switch
    {
        CampaignStatus.Aktywna => "AKT",
        CampaignStatus.Wygasła => "WYG",
        _ => UiKit.Clip(status.ToString(), 3)
    };
}

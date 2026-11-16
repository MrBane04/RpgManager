using System.Collections.ObjectModel;
using RpgManager.Core.Models;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace RpgManager.Console;

internal sealed partial class ManagerWindow
{
    private void BuildSessions()
    {
        var campaign = GetCampaignForSection("sesji");
        if (campaign is null)
        {
            CampaignRequiredState("sesjami");
            return;
        }

        var (listPane, detailPane, list) = CreateManagementShell($"SESJE  /  {campaign.Name}", "SZCZEGÓŁY SESJI", out var campaignPicker);
        campaignPicker.Accepted += (_, _) =>
        {
            var selected = ChooseCampaign("Wybierz kampanię sesji");
            if (selected is null) return;
            _activeCampaignId = selected.Id;
            Navigate("Sesje");
        };

        const int pageSize = 10;
        var page = 1;
        var totalCount = _sessionService.CountSessions(campaign.Id);
        var pageCount = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        IReadOnlyList<Session> sessions = [];
        ListView? detailActions = null;

        var pageInfo = UiKit.Label("", 1, Pos.AnchorEnd(2), Dim.Fill(2));
        var pageOptions = new List<string> { "+ Dodaj sesję" };
        var pageActions = UiKit.List(pageOptions, 1, Pos.AnchorEnd(5), Dim.Fill(2), 3);
        pageActions.SelectedItem = 0;
        pageActions.Accepted += (_, _) =>
        {
            if (pageActions.SelectedItem is not int actionIndex || actionIndex < 0 || actionIndex >= pageOptions.Count)
                return;

            switch (pageOptions[actionIndex])
            {
                case "+ Dodaj sesję": AddSession(campaign); break;
                case "Poprzednia": page--; LoadPage(); break;
                case "Następna": page++; LoadPage(); break;
            }
        };
        listPane.Add(pageInfo, pageActions);
        void RefreshFocusOrder()
        {
            if (detailActions is null)
                SetFocusOrder(_navigation, list, pageActions, campaignPicker);
            else
                SetFocusOrder(_navigation, list, detailActions, pageActions, campaignPicker);
        }

        void RefreshPageActions()
        {
            pageOptions.Clear();
            pageOptions.Add("+ Dodaj sesję");
            if (page > 1) pageOptions.Add("Poprzednia");
            if (page < pageCount) pageOptions.Add("Następna");
            pageActions.SetSource(new ObservableCollection<string>(pageOptions));
            pageActions.SelectedItem = 0;
        }

        void LoadPage()
        {
            sessions = _sessionService.GetSessions(campaign.Id, page, pageSize);
            list.SetSource(new ObservableCollection<string>(sessions.Select(s =>
                $"{s.SessionDate.ToLocalTime():dd.MM HH:mm}  {UiKit.Clip(s.Title, 8)}").ToList()));
            if (sessions.Count > 0)
                list.SelectedItem = 0;
            pageInfo.Text = $"{page}/{pageCount}  |  {totalCount} ses.";
            RefreshPageActions();
            Render(list.SelectedItem);
        }

        void Render(int? selected)
        {
            detailPane.RemoveAll();
            detailActions = null;
            if (selected is not int index || index < 0 || index >= sessions.Count)
            {
                detailPane.Add(UiKit.Label(totalCount == 0
                    ? "Ta kampania nie ma jeszcze sesji. Wybierz Dodaj sesję w dolnym menu listy, aby zaplanować pierwszą."
                    : "Wybierz sesję z listy. Jej termin, opis i dostępne działania pojawią się tutaj.", 1, 1, Dim.Fill(2), 3));
                RefreshFocusOrder();
                return;
            }

            var session = sessions[index];
            detailPane.Title = session.Title;
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Termin", session.SessionDate.ToLocalTime().ToString("dd.MM.yyyy HH:mm")), 1, 1, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Status", UiKit.SessionStatusLabel(session.Status)), 1, 2, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Czas", session.DurationMinutes is int duration ? $"{duration} minut" : "nieokreślony"), 1, 3, Dim.Fill(2)));
            detailPane.Add(UiKit.Label("OPIS", 1, 5, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.Wrap(session.Description, 28, 2), 1, 6, Dim.Fill(2), 2));
            detailPane.Add(UiKit.Label("OPERACJE  /  ENTER", 1, 8, Dim.Fill(2)));
            var actions = UiKit.List(["Edytuj sesję", "Usuń sesję"], 1, 9, Dim.Fill(2), 2);
            detailActions = actions;
            actions.SelectedItem = 0;
            actions.Accepted += (_, _) =>
            {
                if (actions.SelectedItem == 0) EditSession(campaign, session);
                else if (actions.SelectedItem == 1) DeleteSession(campaign, session);
            };
            detailPane.Add(actions);
            RefreshFocusOrder();
        }

        list.ValueChanged += (_, args) => Render(args.NewValue);
        LoadPage();
    }

    private void BuildCharacters()
    {
        var campaign = GetCampaignForSection("postaci");
        if (campaign is null)
        {
            CampaignRequiredState("postaciami");
            return;
        }

        var (listPane, detailPane, list) = CreateManagementShell($"POSTACIE  /  {campaign.Name}", "KARTA POSTACI", out var campaignPicker);
        campaignPicker.Accepted += (_, _) =>
        {
            var selected = ChooseCampaign("Wybierz kampanię postaci");
            if (selected is null) return;
            _activeCampaignId = selected.Id;
            Navigate("Postacie");
        };

        const int pageSize = 10;
        var page = 1;
        var totalCount = _characterService.CountCharacters(campaign.Id);
        var pageCount = Math.Max(1, (int)Math.Ceiling(totalCount / (double)pageSize));
        IReadOnlyList<Character> characters = [];
        ListView? detailActions = null;

        var pageInfo = UiKit.Label("", 1, Pos.AnchorEnd(2), Dim.Fill(2));
        var pageOptions = new List<string> { "+ Dodaj postać" };
        var pageActions = UiKit.List(pageOptions, 1, Pos.AnchorEnd(5), Dim.Fill(2), 3);
        pageActions.SelectedItem = 0;
        pageActions.Accepted += (_, _) =>
        {
            if (pageActions.SelectedItem is not int actionIndex || actionIndex < 0 || actionIndex >= pageOptions.Count)
                return;

            switch (pageOptions[actionIndex])
            {
                case "+ Dodaj postać": AddCharacter(campaign); break;
                case "Poprzednia": page--; LoadPage(); break;
                case "Następna": page++; LoadPage(); break;
            }
        };
        listPane.Add(pageInfo, pageActions);
        void RefreshFocusOrder()
        {
            if (detailActions is null)
                SetFocusOrder(_navigation, list, pageActions, campaignPicker);
            else
                SetFocusOrder(_navigation, list, detailActions, pageActions, campaignPicker);
        }

        void RefreshPageActions()
        {
            pageOptions.Clear();
            pageOptions.Add("+ Dodaj postać");
            if (page > 1) pageOptions.Add("Poprzednia");
            if (page < pageCount) pageOptions.Add("Następna");
            pageActions.SetSource(new ObservableCollection<string>(pageOptions));
            pageActions.SelectedItem = 0;
        }

        void LoadPage()
        {
            characters = _characterService.GetCharacters(campaign.Id, page, pageSize);
            list.SetSource(new ObservableCollection<string>(characters.Select(c =>
                $"{UiKit.Clip(c.Name, 14),-14}  P{c.Level}").ToList()));
            if (characters.Count > 0)
                list.SelectedItem = 0;
            pageInfo.Text = $"{page}/{pageCount}  |  {totalCount}";
            RefreshPageActions();
            Render(list.SelectedItem);
        }

        void Render(int? selected)
        {
            detailPane.RemoveAll();
            detailActions = null;
            if (selected is not int index || index < 0 || index >= characters.Count)
            {
                detailPane.Add(UiKit.Label(totalCount == 0
                    ? "Ta kampania nie ma jeszcze postaci. Wybierz Dodaj postać w dolnym menu listy, aby utworzyć pierwszą kartę."
                    : "Wybierz postać, aby zobaczyć jej kartę i działania.", 1, 1, Dim.Fill(2), 2));
                RefreshFocusOrder();
                return;
            }

            var character = characters[index];
            detailPane.Title = character.Name;
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Poziom", character.Level.ToString()), 1, 1, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Rasa", character.Race), 1, 2, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Klasa", character.CharacterClass), 1, 3, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.DetailLine("Wiek", character.Age.ToString()), 1, 4, Dim.Fill(2)));
            detailPane.Add(UiKit.Label("OPIS POSTACI", 1, 6, Dim.Fill(2)));
            detailPane.Add(UiKit.Label(UiKit.Wrap(character.Description, 28, 2), 1, 7, Dim.Fill(2), 2));
            var actions = UiKit.List(["Edytuj kartę", "Usuń postać"], 1, 9, Dim.Fill(2), 2);
            detailActions = actions;
            actions.SelectedItem = 0;
            actions.Accepted += (_, _) =>
            {
                if (actions.SelectedItem == 0) EditCharacter(campaign, character);
                else if (actions.SelectedItem == 1) DeleteCharacter(campaign, character);
            };
            detailPane.Add(actions);
            RefreshFocusOrder();
        }

        list.ValueChanged += (_, args) => Render(args.NewValue);
        LoadPage();
    }

    private (FrameView ListPane, FrameView DetailPane, ListView List) CreateManagementShell(
        string listTitle, string detailTitle, out ListView campaignPicker)
    {
        var context = UiKit.Panel(listTitle, 0, 0, Dim.Fill(), 3);
        campaignPicker = UiKit.List(["Zmień kampanię"], 1, 0, Dim.Fill(2), 1);
        campaignPicker.SelectedItem = 0;
        context.Add(campaignPicker);
        var listPane = UiKit.Panel("LISTA", 0, 3, Dim.Percent(42), Dim.Fill());
        var detailPane = UiKit.Panel(detailTitle, Pos.Right(listPane), 3, Dim.Fill(), Dim.Fill());
        var list = UiKit.List(Array.Empty<string>(), 1, 0, Dim.Fill(2), Dim.Fill(7));
        listPane.Add(list);
        _workspace.Add(context, listPane, detailPane);
        return (listPane, detailPane, list);
    }

    private Campaign? GetCampaignForSection(string subject)
    {
        var campaigns = _campaignService.GetCampaigns();
        if (campaigns.Count == 0)
            return null;

        var active = campaigns.FirstOrDefault(c => c.Id == _activeCampaignId);
        if (active is not null)
            return active;

        if (campaigns.Count == 1)
        {
            _activeCampaignId = campaigns[0].Id;
            return campaigns[0];
        }

        var selected = ChooseCampaign($"Wybierz kampanię dla {subject}");
        _activeCampaignId = selected?.Id;
        return selected;
    }

    private Campaign? ChooseCampaign(string title)
    {
        var campaigns = _campaignService.GetCampaigns();
        if (campaigns.Count == 0)
            return null;

        var dialog = new ChoiceDialog(title, campaigns.Select(c =>
            $"{c.Name}  |  {c.System}  |  {c.Status}").ToArray());
        App!.Run(dialog);
        return dialog.SelectedIndex is int index && index >= 0 && index < campaigns.Count
            ? campaigns[index]
            : null;
    }

    private void EmptyState(string message)
    {
        var panel = UiKit.Panel("BRAK DANYCH", 1, 1, Dim.Fill(2), 7);
        panel.Add(UiKit.Label(message, 2, 1, Dim.Fill(4), 2));
        var create = UiKit.List(["Utwórz kampanię"], 2, 4, Dim.Fill(4), 1);
        create.SelectedItem = 0;
        create.Accepted += (_, _) => AddCampaign();
        panel.Add(create);
        _workspace.Add(panel);
        SetFocusOrder(_navigation, create);
    }

    private void CampaignRequiredState(string subject)
    {
        if (_campaignService.GetCampaigns().Count == 0)
        {
            EmptyState($"Najpierw utwórz kampanię, aby zarządzać {subject}.");
            return;
        }

        var panel = UiKit.Panel("WYBÓR KAMPANII", 1, 1, Dim.Fill(2), 7);
        panel.Add(UiKit.Label($"Nie wybrano kampanii dla {subject}.", 2, 1, Dim.Fill(4), 2));
        var choose = UiKit.List(["Wybierz kampanię"], 2, 4, Dim.Fill(4), 1);
        choose.SelectedItem = 0;
        choose.Accepted += (_, _) =>
        {
            var selected = ChooseCampaign("Wybierz kampanię");
            if (selected is null) return;
            _activeCampaignId = selected.Id;
            Navigate(_section);
        };
        panel.Add(choose);
        _workspace.Add(panel);
        SetFocusOrder(_navigation, choose);
    }
}

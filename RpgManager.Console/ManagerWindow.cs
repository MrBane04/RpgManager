using RpgManager.Core.Models;
using RpgManager.Core.Services;
using Terminal.Gui.Editor;
using System.Collections.ObjectModel;
using Terminal.Gui.App;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace RpgManager.Console;

/// <summary>
/// Persistent TUI shell. This layer composes presentation views and calls the existing
/// application services; it does not contain or replace domain rules.
/// </summary>
internal sealed partial class ManagerWindow : Window
{
    private readonly CampaignService _campaignService;
    private readonly SessionService _sessionService;
    private readonly CharacterService _characterService;
    private readonly NoteService _noteService;
    private readonly FocusRing _focusRing = new();
    private readonly View _workspace;
    private readonly ListView _navigation;
    private readonly Label _breadcrumb;
    private FrameView? _notePane;
    private Label? _noteStatus;
    private string _section = "Pulpit";
    private int? _activeCampaignId;
    private Editor? _noteEditor;
    private string _originalNote = string.Empty;
    private int? _editingNoteCampaignId;
    private bool _restoringNoteSelection;
    private bool _syncingNavigation;

    private static readonly string[] Sections = ["Pulpit", "Kampanie", "Sesje", "Postacie", "Kronika"];

    public ManagerWindow(CampaignService campaignService, SessionService sessionService,
        CharacterService characterService, NoteService noteService)
    {
        _campaignService = campaignService;
        _sessionService = sessionService;
        _characterService = characterService;
        _noteService = noteService;

        Title = "RPG Manager";
        SchemeName = "RpgBase";
        CanFocus = true;
        TabStop = TabBehavior.TabGroup;

        var masthead = new FrameView
        {
            X = 0, Y = 0, Width = Dim.Fill(), Height = 3,
            Title = "RPG MANAGER  /  PANEL PROWADZĄCEGO",
            SchemeName = "RpgPanel"
        };
        masthead.Add(new Label
        {
            X = 2, Y = 0, Width = Dim.Fill(4), Height = 1,
            Text = "KAMPANIE   /   SESJE   /   POSTACIE   /   KRONIKA",
            SchemeName = "RpgBase"
        });

        var navigationPane = UiKit.Panel("NAWIGACJA", 0, 3, Dim.Percent(22), Dim.Fill(1));
        _navigation = new ListView
        {
            X = 0, Y = 0, Width = Dim.Fill(), Height = Dim.Fill(),
            SchemeName = "RpgMenu",
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };
        _navigation.SetSource(new ObservableCollection<string>(Sections
            .Select((name, index) => $"  {index + 1}   {name}")
            .Append("  6   Wyjście")));
        _navigation.Accepted += (_, _) =>
        {
            if (_syncingNavigation || _navigation.SelectedItem is not int index)
                return;
            if (index < Sections.Length)
                Navigate(Sections[index]);
            else if (index == Sections.Length)
                RequestExit();
        };
        navigationPane.Add(_navigation);

        _breadcrumb = new Label
        {
            X = Pos.Right(navigationPane) + 2, Y = 3, Width = Dim.Fill(2), Height = 1,
            Text = "Pulpit",
            SchemeName = "RpgBase"
        };
        _workspace = new View
        {
            X = Pos.Right(navigationPane) + 2, Y = 4, Width = Dim.Fill(2), Height = Dim.Fill(2),
            SchemeName = "RpgBase",
            CanFocus = true,
            TabStop = TabBehavior.TabGroup
        };
        var footer = new Label
        {
            X = 0, Y = Pos.AnchorEnd(1), Width = Dim.Fill(), Height = 1,
            Text = " ↑/↓ wybór   Enter otwiera   Tab następny obszar   Shift+Tab poprzedni   Ctrl+S zapis   Esc pulpit   Ctrl+Q wyjście ",
            SchemeName = "RpgPanel"
        };

        Add(masthead, navigationPane, _breadcrumb, _workspace, footer);
        KeyBindings.Add(Key.Q.WithCtrl, Command.Quit);
        KeyBindings.Add(Key.S.WithCtrl, Command.Save);
        KeyBindings.Add(Key.R.WithCtrl, Command.Refresh);
        KeyBindings.Add(Key.Esc, Command.Cancel);
        KeyBindings.Add(Key.Tab, Command.NextTabGroup);
        KeyBindings.Add(Key.Tab.WithShift, Command.PreviousTabGroup);
        AddCommand(Command.Quit, () =>
        {
            RequestExit();
            return true;
        });
        AddCommand(Command.Cancel, () =>
        {
            Navigate("Pulpit");
            return true;
        });
        AddCommand(Command.Save, () =>
        {
            if (_section != "Kronika") return false;
            SaveNote();
            return true;
        });
        AddCommand(Command.Refresh, () =>
        {
            if (_section != "Kronika") return false;
            RestoreNote();
            return true;
        });
        AddCommand(Command.NextTabGroup, () =>
        {
            return _focusRing.Move(NavigationDirection.Forward);
        });
        AddCommand(Command.PreviousTabGroup, () =>
        {
            return _focusRing.Move(NavigationDirection.Backward);
        });
        Navigate("Pulpit");
    }

    private void Navigate(string section)
    {
        if (_section == "Kronika" && section != "Kronika" && !ConfirmDiscardNote())
        {
            _syncingNavigation = true;
            _navigation.SelectedItem = Array.IndexOf(Sections, "Kronika");
            _syncingNavigation = false;
            return;
        }

        _section = section;
        if (section != "Kronika")
        {
            _noteEditor = null;
            _notePane = null;
            _noteStatus = null;
            _editingNoteCampaignId = null;
        }
        var campaign = _activeCampaignId is int id
            ? _campaignService.GetCampaigns().FirstOrDefault(c => c.Id == id)
            : null;
        _breadcrumb.Text = campaign is null ? section : $"{section}   /   {campaign.Name}";
        _workspace.RemoveAll();

        switch (section)
        {
            case "Pulpit": BuildDashboard(); break;
            case "Kampanie": BuildCampaigns(); break;
            case "Sesje": BuildSessions(); break;
            case "Postacie": BuildCharacters(); break;
            case "Kronika": BuildNotes(); break;
        }

        var destination = Array.IndexOf(Sections, section);
        if (_navigation.SelectedItem != destination)
        {
            _syncingNavigation = true;
            _navigation.SelectedItem = destination;
            _syncingNavigation = false;
        }

        // Section content is rebuilt above. Return the keyboard to the stable
        // navigation list so context-menu navigation never leaves focus detached.
        _navigation.SetFocus();
        _focusRing.Focus(_navigation);
    }

    private void SetFocusOrder(params View[] targets) => _focusRing.SetTargets(targets);

    private bool ConfirmDiscardNote()
    {
        if (_noteEditor is null || _noteEditor.Text.ToString() == _originalNote)
            return true;

        return Confirm("Niezapisane zmiany",
            "Kronika zawiera niezapisane zmiany. Odrzucić je?", "Odrzuć", "Wróć");
    }

    private void RequestExit()
    {
        if (!ConfirmDiscardNote())
            return;

        App!.RequestStop();
    }

    private bool Confirm(string title, string message, string confirmText, string cancelText)
    {
        var dialog = new ConfirmDialog(title, message, confirmText, cancelText);
        App!.Run(dialog);
        return dialog.Confirmed;
    }

    private void ShowMessage(string title, string message)
    {
        App!.Run(new NoticeDialog(title, message));
    }

    private void ShowError(string message)
    {
        App!.Run(new NoticeDialog("Błąd", message, isError: true));
    }
}

using RpgManager.Core.Models;
using Terminal.Gui.Editor;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace RpgManager.Console;

internal sealed partial class ManagerWindow
{
    private void BuildNotes()
    {
        var campaigns = _campaignService.GetCampaigns();
        if (campaigns.Count == 0)
        {
            EmptyState("Utwórz kampanię, aby otrzymać jej kronikę.");
            return;
        }

        var campaignPane = UiKit.Panel("KAMPANIE", 0, 0, Dim.Percent(32), Dim.Fill());
        var editorPane = UiKit.Panel("KRONIKA", Pos.Right(campaignPane), 0, Dim.Fill(), Dim.Fill());
        _notePane = editorPane;
        _workspace.Add(campaignPane, editorPane);

        var rows = campaigns.Select(c => UiKit.Clip(c.Name, 14)).ToList();
        var list = UiKit.List(rows, 1, 0, Dim.Fill(2), Dim.Fill());
        list.SelectedItem = 0;
        campaignPane.Add(list);

        void Load(int? selected)
        {
            if (_restoringNoteSelection)
                return;

            var targetId = selected is int selection && selection >= 0 && selection < campaigns.Count
                ? campaigns[selection].Id
                : (int?)null;
            if (_noteEditor is not null && _editingNoteCampaignId != targetId && !ConfirmDiscardNote())
            {
                var previous = campaigns.ToList().FindIndex(c => c.Id == _editingNoteCampaignId);
                if (previous >= 0)
                {
                    _restoringNoteSelection = true;
                    list.SelectedItem = previous;
                    _restoringNoteSelection = false;
                }
                return;
            }

            editorPane.RemoveAll();
            _noteEditor = null;
            _editingNoteCampaignId = null;
            if (selected is not int index || index < 0 || index >= campaigns.Count)
            {
                SetFocusOrder(_navigation, list);
                return;
            }

            var campaign = campaigns[index];
            _activeCampaignId = campaign.Id;
            _breadcrumb.Text = $"Kronika   /   {campaign.Name}";
            var note = _noteService.GetNote(campaign.Id);
            editorPane.Title = note is null ? $"{campaign.Name}  /  KRONIKA" : $"{campaign.Name}  /  {note.UpdatedAt.ToLocalTime():dd.MM.yyyy HH:mm}";

            if (note is null)
            {
                editorPane.Add(UiKit.Label("Nie znaleziono notatki przypisanej do tej kampanii.", 2, 1, Dim.Fill(4), 2));
                SetFocusOrder(_navigation, list);
                return;
            }

            _editingNoteCampaignId = campaign.Id;
            _originalNote = note.Content;
            var editor = new Editor
            {
                X = 1, Y = 0, Width = Dim.Fill(2), Height = Dim.Fill(6),
                Text = note.Content,
                WordWrap = true,
                GutterOptions = GutterOptions.LineNumbers,
                ViewportSettings = ViewportSettingsFlags.HasScrollBars,
                SchemeName = "RpgInput",
                CanFocus = true,
                TabStop = TabBehavior.TabStop
            };
            _noteEditor = editor;
            editor.KeyBindings.Remove(Key.Tab);
            editor.KeyBindings.Remove(Key.Tab.WithShift);
            _noteStatus = UiKit.Label("", 1, Pos.AnchorEnd(4), Dim.Fill(2));
            editor.Document!.TextChanged += (_, _) => UpdateNoteStatus();
            editorPane.Add(editor);
            editorPane.Add(_noteStatus);
            editorPane.Add(UiKit.Label("Ctrl+S zapisz   Ctrl+R przywróć   Esc", 1,
                Pos.AnchorEnd(2), Dim.Fill(2)));
            UpdateNoteStatus();
            SetFocusOrder(_navigation, list, editor);
        }

        list.ValueChanged += (_, args) => Load(args.NewValue);
        Load(list.SelectedItem);
    }

    private void SaveNote()
    {
        if (_editingNoteCampaignId is not int campaignId || _noteEditor is null)
            return;

        var content = _noteEditor.Text.ToString();
        var result = _noteService.UpdateNote(campaignId, content);
        if (!result.Success)
        {
            ShowError(result.Message);
            return;
        }

        _originalNote = content;
        UpdateNoteStatus();
        var note = _noteService.GetNote(campaignId);
        if (note is not null && _notePane is not null)
            _notePane.Title = $"{note.UpdatedAt.ToLocalTime():dd.MM.yyyy HH:mm}  /  KRONIKA";
        ShowMessage("Zapisano kronikę", result.Message);
    }

    private void RestoreNote()
    {
        if (_noteEditor is null)
            return;

        if (_noteEditor.Text.ToString() != _originalNote && !ConfirmDiscardNote())
            return;

        _noteEditor.Text = _originalNote;
        UpdateNoteStatus();
    }

    private void UpdateNoteStatus()
    {
        if (_noteStatus is null || _noteEditor is null)
            return;

        var text = _noteEditor.Text.ToString();
        var lineCount = text.Length == 0 ? 1 : text.Count(character => character == '\n') + 1;
        var saveState = text == _originalNote ? "Zapisano" : "Niezapisane zmiany";
        _noteStatus.Text = $"{saveState}  |  {lineCount} w.  |  {text.Length} znaków";
    }
}

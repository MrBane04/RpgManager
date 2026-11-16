using System.Collections.ObjectModel;
using Terminal.Gui.App;
using Terminal.Gui.Editor;
using Terminal.Gui.Input;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace RpgManager.Console;

internal sealed record FormField(string Label, string Value, bool Multiline = false);

internal static class DialogMetrics
{
    public static (int Width, int Height) TerminalSize
    {
        get
        {
            try
            {
                return (Math.Max(1, System.Console.WindowWidth), Math.Max(1, System.Console.WindowHeight));
            }
            catch (IOException)
            {
                return (80, 24);
            }
            catch (PlatformNotSupportedException)
            {
                return (80, 24);
            }
        }
    }
}

internal abstract class KeyboardDialog : Dialog
{
    protected void EnableTabNavigation()
    {
        KeyBindings.Add(Key.Tab, Command.NextTabStop);
        KeyBindings.Add(Key.Tab.WithShift, Command.PreviousTabStop);
        AddCommand(Command.NextTabStop, () =>
        {
            AdvanceFocus(NavigationDirection.Forward, TabBehavior.TabStop);
            return true;
        });
        AddCommand(Command.PreviousTabStop, () =>
        {
            AdvanceFocus(NavigationDirection.Backward, TabBehavior.TabStop);
            return true;
        });
    }
}

/// <summary>A reusable keyboard-first editor with inline validation and explicit shortcuts.</summary>
internal sealed class InputDialog : KeyboardDialog
{
    private readonly FormField[] _definitions;
    private readonly View[] _inputs;
    private readonly Label _error;
    private readonly Func<string[], string?> _validate;

    public bool Saved { get; private set; }
    public string[] Values { get; private set; } = [];

    public InputDialog(string title, string subtitle, FormField[] fields, Func<string[], string?> validate)
    {
        _definitions = fields;
        _inputs = new View[fields.Length];
        _validate = validate;

        Title = title;
        SchemeName = "RpgPanel";
        CanFocus = true;
        TabStop = TabBehavior.TabGroup;
        var terminalSize = DialogMetrics.TerminalSize;
        var terminalWidth = Math.Max(48, terminalSize.Width - 4);
        var desiredWidth = Math.Max(64, fields.Max(field => field.Label.Length) + 42);
        var dialogWidth = Math.Min(92, Math.Min(terminalWidth, desiredWidth));
        Width = dialogWidth;
        Height = Math.Min(EstimateHeight(fields), Math.Max(10, terminalSize.Height - 2));

        var labelWidth = Math.Min(27, Math.Max(15, dialogWidth / 3));
        var inputX = labelWidth + 3;
        var row = 2;
        Add(UiKit.Label(UiKit.Wrap(subtitle, dialogWidth - 10, 2), 2, 0, Dim.Fill(4), 2));

        foreach (var (field, index) in fields.Select((field, index) => (field, index)))
        {
            Add(UiKit.Label(field.Label, 2, row, labelWidth));
            if (field.Multiline)
            {
                var editor = new Editor
                {
                    X = inputX, Y = row, Width = Dim.Fill(4), Height = 3,
                    Text = field.Value,
                    WordWrap = true,
                    GutterOptions = GutterOptions.None,
                    SchemeName = "RpgInput",
                    CanFocus = true,
                    TabStop = TabBehavior.TabStop
                };
                editor.KeyBindings.Remove(Key.Tab);
                editor.KeyBindings.Remove(Key.Tab.WithShift);
                _inputs[index] = editor;
                Add(editor);
                row += 4;
            }
            else
            {
                var input = new TextField
                {
                    X = inputX, Y = row, Width = Dim.Fill(4), Height = 1,
                    Text = field.Value,
                    SchemeName = "RpgInput",
                    CanFocus = true,
                    TabStop = TabBehavior.TabStop
                };
                _inputs[index] = input;
                Add(input);
                row += 2;
            }
        }

        _error = UiKit.Label("", 2, row, Dim.Fill(4), 2);
        _error.SchemeName = "RpgDanger";
        Add(_error);
        Add(UiKit.Label("Tab następne pole   Ctrl+S zapisz   Esc anuluj", 2, Pos.AnchorEnd(2), Dim.Fill(4)));
        if (_inputs.Length > 0)
            _inputs[0].SetFocus();

        KeyBindings.Add(Key.S.WithCtrl, Command.Save);
        KeyBindings.Add(Key.Esc, Command.Cancel);
        EnableTabNavigation();
        AddCommand(Command.Save, () =>
        {
            TrySave();
            return true;
        });
        AddCommand(Command.Cancel, () =>
        {
            App!.RequestStop();
            return true;
        });
    }

    private static int EstimateHeight(IEnumerable<FormField> fields) =>
        8 + fields.Sum(field => field.Multiline ? 4 : 2);

    private void TrySave()
    {
        Values = _inputs.Select((input, index) => input switch
        {
            TextField field => field.Text?.ToString() ?? string.Empty,
            Editor editor => editor.Text?.ToString() ?? string.Empty,
            _ => _definitions[index].Value
        }).ToArray();

        _error.Text = _validate(Values) ?? string.Empty;
        if (!string.IsNullOrEmpty(_error.Text))
            return;

        Saved = true;
        App!.RequestStop();
    }
}

/// <summary>A compact, scrollable picker for campaign context.</summary>
internal sealed class ChoiceDialog : KeyboardDialog
{
    private readonly ListView _choices;

    public int? SelectedIndex { get; private set; }

    public ChoiceDialog(string title, IReadOnlyList<string> choices)
    {
        Title = title;
        SchemeName = "RpgPanel";
        CanFocus = true;
        TabStop = TabBehavior.TabGroup;
        var terminalSize = DialogMetrics.TerminalSize;
        var dialogWidth = Math.Min(76, Math.Max(42, terminalSize.Width - 4));
        Width = dialogWidth;
        Height = Math.Min(Math.Min(18, choices.Count + 8), Math.Max(10, terminalSize.Height - 4));
        Add(UiKit.Label("Wybierz kampanię strzałkami, zatwierdź Enterem.", 2, 0, Dim.Fill(4)));
        _choices = UiKit.List(choices, 1, 2, Dim.Fill(3), Dim.Fill(4));
        if (choices.Count > 0)
            _choices.SelectedItem = 0;
        _choices.Accepted += (_, _) => Choose();
        Add(_choices);
        _choices.SetFocus();
        Add(UiKit.Label("Enter wybierz   Esc wróć", 2, Pos.AnchorEnd(2), Dim.Fill(4)));
        EnableTabNavigation();
        KeyBindings.Add(Key.Esc, Command.Cancel);
        AddCommand(Command.Accept, () =>
        {
            Choose();
            return true;
        });
        AddCommand(Command.Cancel, () =>
        {
            App!.RequestStop();
            return true;
        });
    }

    private void Choose()
    {
        SelectedIndex = _choices.SelectedItem;
        if (SelectedIndex is not null)
            App!.RequestStop();
    }
}

/// <summary>Visible list-based confirmation so actions remain clear without mouse input.</summary>
internal sealed class ConfirmDialog : KeyboardDialog
{
    private readonly ListView _choices;

    public bool Confirmed { get; private set; }

    public ConfirmDialog(string title, string message, string confirmText, string cancelText)
    {
        Title = title;
        SchemeName = "RpgAccent";
        CanFocus = true;
        TabStop = TabBehavior.TabGroup;
        var terminalSize = DialogMetrics.TerminalSize;
        var dialogWidth = Math.Min(64, Math.Max(42, terminalSize.Width - 4));
        Width = dialogWidth;
        Height = Math.Min(9, Math.Max(8, terminalSize.Height - 4));
        Add(UiKit.Label(UiKit.Wrap(message, dialogWidth - 10, 3), 2, 1, Dim.Fill(4), 3));
        _choices = UiKit.List([$"  {cancelText}", $"  {confirmText}"], 2, 5, Dim.Fill(4), 2);
        _choices.SelectedItem = 0;
        _choices.Accepted += (_, _) => Choose();
        Add(_choices);
        _choices.SetFocus();
        Add(UiKit.Label("↑/↓ wybierz   Enter zatwierdź   Esc anuluj", 2, Pos.AnchorEnd(2), Dim.Fill(4)));
        EnableTabNavigation();
        KeyBindings.Add(Key.Esc, Command.Cancel);
        AddCommand(Command.Accept, () =>
        {
            Choose();
            return true;
        });
        AddCommand(Command.Cancel, () =>
        {
            App!.RequestStop();
            return true;
        });
    }

    private void Choose()
    {
        Confirmed = _choices.SelectedItem == 1;
        App!.RequestStop();
    }
}

/// <summary>Simple, consistent acknowledgement dialog that needs no invisible buttons.</summary>
internal sealed class NoticeDialog : KeyboardDialog
{
    private readonly ListView _dismiss;

    public NoticeDialog(string title, string message, bool isError = false)
    {
        Title = title;
        CanFocus = true;
        TabStop = TabBehavior.TabGroup;
        var terminalSize = DialogMetrics.TerminalSize;
        var dialogWidth = Math.Min(64, Math.Max(42, terminalSize.Width - 4));
        Width = dialogWidth;
        Height = Math.Min(8, Math.Max(7, terminalSize.Height - 4));
        SchemeName = isError ? "RpgDanger" : "RpgPanel";
        Add(UiKit.Label(UiKit.Wrap(message, dialogWidth - 10, 2), 2, 1, Dim.Fill(4), 2));
        _dismiss = UiKit.List(["OK"], 2, 4, Dim.Fill(4), 1);
        _dismiss.SelectedItem = 0;
        _dismiss.Accepted += (_, _) => App!.RequestStop();
        Add(_dismiss);
        _dismiss.SetFocus();
        Add(UiKit.Label("Enter lub Esc zamknij", 2, Pos.AnchorEnd(2), Dim.Fill(4)));
        EnableTabNavigation();
        KeyBindings.Add(Key.Esc, Command.Cancel);
        AddCommand(Command.Accept, () =>
        {
            App!.RequestStop();
            return true;
        });
        AddCommand(Command.Cancel, () =>
        {
            App!.RequestStop();
            return true;
        });
    }
}

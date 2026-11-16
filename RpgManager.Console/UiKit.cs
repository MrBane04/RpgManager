using System.Collections.ObjectModel;
using RpgManager.Core.Models;
using Terminal.Gui.Drawing;
using Terminal.Gui.ViewBase;
using Terminal.Gui.Views;

namespace RpgManager.Console;

/// <summary>Small set of repeatable, data-first building blocks for the terminal UI.</summary>
internal static class UiKit
{
    public static FrameView Panel(string title, Pos x, Pos y, Dim width, Dim height, string schemeName = "RpgPanel") => new()
    {
        X = x, Y = y, Width = width, Height = height,
        Title = title,
        SchemeName = schemeName,
        BorderStyle = LineStyle.Single,
        CanFocus = true,
        TabStop = TabBehavior.TabGroup
    };

    public static Label Label(string text, Pos x, Pos y, Dim? width = null, Dim? height = null) => new()
    {
        X = x, Y = y,
        Width = width ?? Dim.Auto(),
        Height = height ?? 1,
        SchemeName = "RpgBase",
        Text = text
    };

    public static ListView List(IEnumerable<string> items, Pos x, Pos y, Dim width, Dim height)
    {
        var list = new ListView
        {
            X = x, Y = y, Width = width, Height = height,
            SchemeName = "RpgMenu",
            CanFocus = true,
            TabStop = TabBehavior.TabStop
        };
        list.SetSource(new ObservableCollection<string>(items));
        return list;
    }

    public static FrameView Metric(string caption, string value, Pos x, Pos y, Dim width)
    {
        var panel = new FrameView
        {
            X = x, Y = y, Width = width, Height = 4,
            Title = caption.ToUpperInvariant(),
            SchemeName = "RpgAccent",
            BorderStyle = LineStyle.Single,
            CanFocus = false,
            TabStop = TabBehavior.NoStop
        };
        panel.Add(Label(value, 2, 0, Dim.Fill(3)));
        return panel;
    }

    public static string Clip(string? value, int maxLength)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "—";

        var normalized = value.Replace('\r', ' ').Replace('\n', ' ').Trim();
        return normalized.Length <= maxLength ? normalized : normalized[..Math.Max(0, maxLength - 1)] + "…";
    }

    public static string CampaignLine(string name, string system, string status, int sessions, int characters) =>
        $"{Clip(name, 8),-8} {Clip(system, 5),-5} {StatusShort(status)} S{sessions} P{characters}";

    private static string StatusShort(string status) => status == "Aktywna" ? "AKT" : status == "Wygasła" ? "WYG" : Clip(status, 3);

    public static string DetailLine(string label, string? value, int width = 32) =>
        $"{Clip(label.ToUpperInvariant(), 9),-9} {Clip(value, Math.Max(8, width - 11))}";

    public static string SessionStatusLabel(SessionStatus status) => status switch
    {
        SessionStatus.Planned => "Zaplanowana",
        SessionStatus.Completed => "Zakończona",
        SessionStatus.Canceled => "Odwołana",
        _ => status.ToString()
    };

    public static string Wrap(string? value, int lineWidth, int maxLines)
    {
        if (string.IsNullOrWhiteSpace(value))
            return "Brak opisu.";

        var words = value.Replace('\r', ' ').Replace('\n', ' ')
            .Split(' ', StringSplitOptions.RemoveEmptyEntries);
        var lines = new List<string>();
        var current = string.Empty;
        var truncated = false;

        for (var i = 0; i < words.Length; i++)
        {
            var word = words[i];
            if (word.Length > lineWidth)
            {
                word = word[..lineWidth];
                truncated = true;
            }

            if (current.Length > 0 && current.Length + word.Length + 1 > lineWidth)
            {
                lines.Add(current);
                current = string.Empty;
                if (lines.Count == maxLines)
                {
                    truncated = true;
                    break;
                }
            }

            current = current.Length == 0 ? word : $"{current} {word}";
            if (i == words.Length - 1 && current.Length > 0 && lines.Count < maxLines)
                lines.Add(current);
        }

        if (truncated && lines.Count > 0)
            lines[^1] = lines[^1].TrimEnd() + "…";

        return string.Join(Environment.NewLine, lines);
    }
}

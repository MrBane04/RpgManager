using Terminal.Gui.Configuration;
using Terminal.Gui.Drawing;
using Attribute = Terminal.Gui.Drawing.Attribute;

namespace RpgManager.Console;

/// <summary>One compact, high-contrast palette shared by every TUI screen.</summary>
internal static class UiTheme
{
    public static void Configure()
    {
        var schemes = TuiConfigurationBuilder.Shared.SchemeManager;

        schemes.AddScheme("RpgBase", new Scheme
        {
            Normal = new Attribute("#DDD6C5", "#14171A"),
            Focus = new Attribute("#17191B", "#C5A66A", "Bold"),
            Active = new Attribute("#F4E7C8", "#3A3325", "Bold"),
            Highlight = new Attribute("#E5D7B7", "#202328"),
            Editable = new Attribute("#E9E3D5", "#1B1F23")
        });

        schemes.AddScheme("RpgPanel", new Scheme
        {
            Normal = new Attribute("#C7AD76", "#191C20"),
            Focus = new Attribute("#17191B", "#C5A66A", "Bold"),
            Active = new Attribute("#F4E7C8", "#3A3325", "Bold"),
            Highlight = new Attribute("#D8C28F", "#202328"),
            Editable = new Attribute("#E9E3D5", "#1B1F23")
        });

        schemes.AddScheme("RpgMenu", new Scheme
        {
            Normal = new Attribute("#B8B7B1", "#191C20"),
            Focus = new Attribute("#F2E7CC", "#433A29", "Bold"),
            Active = new Attribute("#F2E7CC", "#433A29", "Bold"),
            Highlight = new Attribute("#DCC58E", "#22262A"),
            Editable = new Attribute("#E9E3D5", "#1B1F23")
        });

        schemes.AddScheme("RpgAccent", new Scheme
        {
            Normal = new Attribute("#D7C38E", "#202329", "Bold"),
            Focus = new Attribute("#17191B", "#C5A66A", "Bold"),
            Active = new Attribute("#F4E7C8", "#3A3325", "Bold"),
            Highlight = new Attribute("#E5D7B7", "#272B30"),
            Editable = new Attribute("#E9E3D5", "#1B1F23")
        });

        schemes.AddScheme("RpgInput", new Scheme
        {
            Normal = new Attribute("#D7D2C7", "#1B1F23"),
            Focus = new Attribute("#F3E8CF", "#292C30"),
            Active = new Attribute("#F3E8CF", "#292C30", "Bold"),
            Highlight = new Attribute("#DCC58E", "#24282D"),
            Editable = new Attribute("#EEE8DA", "#202429")
        });

        schemes.AddScheme("RpgDanger", new Scheme
        {
            Normal = new Attribute("#D99288", "#191C20"),
            Focus = new Attribute("#17191B", "#BF746B", "Bold"),
            Active = new Attribute("#F6E1DD", "#603B39", "Bold"),
            Highlight = new Attribute("#E7B6AF", "#242729"),
            Editable = new Attribute("#EEE8DA", "#202429")
        });
    }
}

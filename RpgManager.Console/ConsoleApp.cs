using RpgManager.Core.Services;
using Terminal.Gui.App;
using System.Text;

namespace RpgManager.Console;

internal sealed class ConsoleApp
{
    private readonly CampaignService _campaigns;
    private readonly SessionService _sessions;
    private readonly CharacterService _characters;
    private readonly NoteService _notes;

    public ConsoleApp(CampaignService campaigns, SessionService sessions, CharacterService characters, NoteService notes)
    {
        _campaigns = campaigns;
        _sessions = sessions;
        _characters = characters;
        _notes = notes;
    }

    public void Run()
    {
        System.Console.InputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        System.Console.OutputEncoding = new UTF8Encoding(encoderShouldEmitUTF8Identifier: false);
        using IApplication app = Application.Create().Init();
        UiTheme.Configure();
        var window = new ManagerWindow(_campaigns, _sessions, _characters, _notes);
        app.Run(window);
    }
}

using RpgManager.Core.Services;
using RpgManager.Console;

CampaignService campaignService = new CampaignService();

ConsoleApp consoleApp = new ConsoleApp(campaignService);

consoleApp.Run();








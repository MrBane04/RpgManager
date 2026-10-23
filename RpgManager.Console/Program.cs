using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RpgManager.Core.Data;
using RpgManager.Core.Services;
using RpgManager.Console;

var configuration = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

var connectionString = configuration.GetConnectionString("DefaultConnection");

var options = new DbContextOptionsBuilder<RpgManagerDbContext>()
    .UseNpgsql(connectionString)
    .Options;

using var dbContext = new RpgManagerDbContext(options);

CampaignService campaignService = new CampaignService();

ConsoleApp consoleApp = new ConsoleApp(campaignService);

consoleApp.Run();








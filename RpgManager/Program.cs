using Microsoft.EntityFrameworkCore;
using RpgManager.Core.Data;
using RpgManager.Core.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddScoped<CampaignService>();

builder.Services.AddDbContext<RpgManagerDbContext>(options =>
    options.UseNpgsql(
        builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddControllers();

builder.Services.AddOpenApi();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();

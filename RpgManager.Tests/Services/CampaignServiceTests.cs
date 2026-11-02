using RpgManager.Core.Data;
using RpgManager.Core.Services;
using Microsoft.EntityFrameworkCore;
using RpgManager.Core.Models;
using Xunit;

public class CampaignServiceTests
{
    private RpgManagerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RpgManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RpgManagerDbContext(options);
    }

    [Fact]
    public void CreateCampaign_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        var result = campaignService.CreateCampaign(
            "Moja kampania",
            "DND",
            CampaignStatus.Aktywna);

        Assert.True(result.Success);
        Assert.Single(context.Campaigns);
    }


    [Fact]
    public void CreateCampaign_InvalidNameData_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        var result = campaignService.CreateCampaign(
            "",
            "DND",
            CampaignStatus.Aktywna);

        Assert.False(result.Success);
        Assert.Empty(context.Campaigns);
    }

    [Fact]
    public void CreateCampaign_InvalidSystemData_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        var result = campaignService.CreateCampaign(
            "Moja kampania",
            "",
            CampaignStatus.Aktywna);

        Assert.False(result.Success);
        Assert.Empty(context.Campaigns);
    }

    [Fact]
    public void CreateCampaign_InvalidStatusData_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        var result = campaignService.CreateCampaign(
            "Moja kampania",
            "DND",
            (CampaignStatus)999);

        Assert.False(result.Success);
        Assert.Empty(context.Campaigns);
    }

    [Fact]
    public void EditCampaign_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        campaignService.CreateCampaign("Stara nazwa", "DND", CampaignStatus.Aktywna);

        var campaign = context.Campaigns.First();

        var result = campaignService.EditCampaign(campaign.Id, "Nowa nazwa", "Jedyny Pierścień", CampaignStatus.Wygasła);

        Assert.True(result.Success);
        Assert.Equal("Nowa nazwa", campaign.Name);
        Assert.Equal("Jedyny Pierścień", campaign.System);
        Assert.Equal(CampaignStatus.Wygasła, campaign.Status);
    }

    [Fact]
    public void EditCampaign_NonExistingId_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        var result = campaignService.EditCampaign(999, "Nowa nazwa", "Jedyny Pierścień", CampaignStatus.Wygasła);

        Assert.False(result.Success);
        Assert.Empty(context.Campaigns);
    }

    [Fact]
    public void EditCampaign_InvalidNameData_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        campaignService.CreateCampaign("Stara nazwa", "DND", CampaignStatus.Aktywna);
        var campaign = context.Campaigns.First();

        var result = campaignService.EditCampaign(
            campaign.Id, "", "DND", CampaignStatus.Aktywna);

        Assert.False(result.Success);
        Assert.Equal("Stara nazwa", campaign.Name);
    }

    [Fact]
    public void EditCampaign_InvalidSystemData_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        campaignService.CreateCampaign("Stara nazwa", "DND", CampaignStatus.Aktywna);
        var campaign = context.Campaigns.First();

        var result = campaignService.EditCampaign(
            campaign.Id, "Nowa nazwa", "", CampaignStatus.Aktywna);

        Assert.False(result.Success);
        Assert.Equal("DND", campaign.System);
    }

    [Fact]
    public void EditCampaign_InvalidStatusData_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        campaignService.CreateCampaign("Stara nazwa", "DND", CampaignStatus.Aktywna);
        var campaign = context.Campaigns.First();

        var result = campaignService.EditCampaign(
            campaign.Id, "Nowa nazwa", "DND", (CampaignStatus)999);

        Assert.False(result.Success);
        Assert.Equal(CampaignStatus.Aktywna, campaign.Status);
    }

    [Fact]
    public void DeleteCampaign_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaign = context.Campaigns.First();
        var result = campaignService.DeleteCampaign(campaign.Id);

        Assert.True(result.Success);
        Assert.Empty(context.Campaigns);
    }

    [Fact]
    public void DeleteCampaign_NonExistantCampaign_ReturnFail()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        var result = campaignService.DeleteCampaign(999);

        Assert.False(result.Success);
        Assert.Empty(context.Campaigns);
    }

    [Fact]
    public void CampaignExists_ExistingId_ReturnTrue()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaign = context.Campaigns.First();

        var exists = campaignService.CampaignExists(campaign.Id);

        Assert.True(exists);
        
    }

    [Fact]
    public void CampaignExists_NonExistingId_ReturnFalse()
    {
        using var context = CreateContext();
        var campaignService = new CampaignService(context);

        Assert.False(campaignService.CampaignExists(999));

    }
}

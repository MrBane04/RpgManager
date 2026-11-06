
using RpgManager.Core.Data;
using RpgManager.Core.Services;
using RpgManager.Core.Models;
using Microsoft.EntityFrameworkCore;
using Xunit;

public class CharacterServiceTests
{
    private RpgManagerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RpgManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RpgManagerDbContext(options);
    }

    // CREATE CHARACTER

    [Fact]
    public void CreateCharacter_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        var result = characterService.CreateCharacter(
            campaignId, "Aragorn", 35, "Człowiek", "Wojownik", 5, "Wędrowiec");

        Assert.True(result.Success);
        Assert.Single(context.Characters);
        Assert.Equal(campaignId, context.Characters.First().CampaignId);
        Assert.Equal("Aragorn", context.Characters.First().Name);
    }

    [Fact]
    public void CreateCharacter_CampaignDoesNotExist_ReturnFailure()
    {
        using var context = CreateContext();
        var characterService = new CharacterService(context);

        var result = characterService.CreateCharacter(
            9999, "Aragorn", 35, "Człowiek", "Wojownik", 5, "Wędrowiec");

        Assert.False(result.Success);
        Assert.Empty(context.Characters);
    }

    [Fact]
    public void CreateCharacter_EmptyName_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        var result = characterService.CreateCharacter(
            campaignId, "", 35, "Człowiek", "Wojownik", 5, "Wędrowiec");

        Assert.False(result.Success);
        Assert.Empty(context.Characters);
    }

    [Fact]
    public void CreateCharacter_InvalidAge_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        var result = characterService.CreateCharacter(
            campaignId, "Aragorn", 0, "Człowiek", "Wojownik", 5, "Wędrowiec");

        Assert.False(result.Success);
        Assert.Empty(context.Characters);
    }

    [Fact]
    public void CreateCharacter_InvalidLevel_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        var result = characterService.CreateCharacter(
            campaignId, "Aragorn", 35, "Człowiek", "Wojownik", 0, "Wędrowiec");

        Assert.False(result.Success);
        Assert.Empty(context.Characters);
    }

    // GET CHARACTERS

    [Fact]
    public void GetCharacters_ReturnCharactersForCampaign()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Kampania 1", "DND", CampaignStatus.Aktywna);
        var campaignId1 = context.Campaigns.First().Id;

        campaignService.CreateCampaign("Kampania 2", "DND", CampaignStatus.Aktywna);
        var campaignId2 = context.Campaigns.Last().Id;

        characterService.CreateCharacter(campaignId1, "Aragorn", 35, null, null, 5, null);
        characterService.CreateCharacter(campaignId1, "Legolas", 100, null, null, 5, null);
        characterService.CreateCharacter(campaignId2, "Gimli", 120, null, null, 5, null);

        var characters = characterService.GetCharacters(campaignId1);

        Assert.Equal(2, characters.Count);
        Assert.All(characters, c => Assert.Equal(campaignId1, c.CampaignId));
    }

    [Fact]
    public void GetCharacters_NoCharacters_ReturnEmptyList()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        var characters = characterService.GetCharacters(campaignId);

        Assert.Empty(characters);
    }

    // GET CHARACTER

    [Fact]
    public void GetCharacter_CharacterExists_ReturnCharacter()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        characterService.CreateCharacter(campaignId, "Aragorn", 35, null, null, 5, null);
        var characterId = context.Characters.First().Id;

        var character = characterService.GetCharacter(campaignId, characterId);

        Assert.NotNull(character);
        Assert.Equal("Aragorn", character.Name);
    }

    [Fact]
    public void GetCharacter_WrongCampaign_ReturnNull()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Kampania 1", "DND", CampaignStatus.Aktywna);
        var campaignId1 = context.Campaigns.First().Id;

        campaignService.CreateCampaign("Kampania 2", "DND", CampaignStatus.Aktywna);
        var campaignId2 = context.Campaigns.Last().Id;

        characterService.CreateCharacter(campaignId1, "Aragorn", 35, null, null, 5, null);
        var characterId = context.Characters.First().Id;

        var character = characterService.GetCharacter(campaignId2, characterId);

        Assert.Null(character);
    }

    // DELETE CHARACTER

    [Fact]
    public void DeleteCharacter_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        characterService.CreateCharacter(campaignId, "Aragorn", 35, null, null, 5, null);
        var characterId = context.Characters.First().Id;

        var result = characterService.DeleteCharacter(campaignId, characterId);

        Assert.True(result.Success);
        Assert.Empty(context.Characters);
    }

    [Fact]
    public void DeleteCharacter_WrongCampaign_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Kampania 1", "DND", CampaignStatus.Aktywna);
        var campaignId1 = context.Campaigns.First().Id;

        campaignService.CreateCampaign("Kampania 2", "DND", CampaignStatus.Aktywna);
        var campaignId2 = context.Campaigns.Last().Id;

        characterService.CreateCharacter(campaignId1, "Aragorn", 35, null, null, 5, null);
        var characterId = context.Characters.First().Id;

        var result = characterService.DeleteCharacter(campaignId2, characterId);

        Assert.False(result.Success);
        Assert.Single(context.Characters);
    }

    // EDIT CHARACTER

    [Fact]
    public void EditCharacter_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        characterService.CreateCharacter(campaignId, "Aragorn", 35, "Człowiek", "Wojownik", 5, "Wędrowiec");
        var characterId = context.Characters.First().Id;

        var result = characterService.EditCharacter(
            campaignId, characterId, "Aragorn II", 36, "Człowiek", "Wojownik", 6, "Król Gondoru");

        var updatedCharacter = context.Characters.First(c => c.Id == characterId);

        Assert.True(result.Success);
        Assert.Equal("Aragorn II", updatedCharacter.Name);
        Assert.Equal(36, updatedCharacter.Age);
        Assert.Equal(6, updatedCharacter.Level);
        Assert.Equal("Król Gondoru", updatedCharacter.Description);
    }

    [Fact]
    public void EditCharacter_CharacterDoesNotExist_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        var result = characterService.EditCharacter(
            campaignId, 999, "Aragorn", 35, null, null, 5, null);

        Assert.False(result.Success);
        Assert.Empty(context.Characters);
    }

    [Fact]
    public void EditCharacter_EmptyName_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        characterService.CreateCharacter(campaignId, "Aragorn", 35, null, null, 5, null);
        var characterId = context.Characters.First().Id;

        var result = characterService.EditCharacter(
            campaignId, characterId, "", 35, null, null, 5, null);

        Assert.False(result.Success);
        Assert.Equal("Aragorn", context.Characters.First().Name);
    }

    [Fact]
    public void EditCharacter_InvalidAge_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        characterService.CreateCharacter(campaignId, "Aragorn", 35, null, null, 5, null);
        var characterId = context.Characters.First().Id;

        var result = characterService.EditCharacter(
            campaignId, characterId, "Aragorn", 0, null, null, 5, null);

        Assert.False(result.Success);
        Assert.Equal(35, context.Characters.First().Age);
    }

    [Fact]
    public void EditCharacter_InvalidLevel_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var characterService = new CharacterService(context);

        campaignService.CreateCampaign("Moja kampania", "DND", CampaignStatus.Aktywna);
        var campaignId = context.Campaigns.First().Id;

        characterService.CreateCharacter(campaignId, "Aragorn", 35, null, null, 5, null);
        var characterId = context.Characters.First().Id;

        var result = characterService.EditCharacter(
            campaignId, characterId, "Aragorn", 35, null, null, 0, null);

        Assert.False(result.Success);
        Assert.Equal(5, context.Characters.First().Level);
    }
}

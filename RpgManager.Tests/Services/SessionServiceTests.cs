using RpgManager.Core.Data;
using RpgManager.Core.Services;
using Microsoft.EntityFrameworkCore;
using RpgManager.Core.Models;
using Xunit;

public class SessionServiceTests
{
    private RpgManagerDbContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<RpgManagerDbContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new RpgManagerDbContext(options);
    }

    [Fact]
    public void CreateSession_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var result = sessionService.CreateSession(
            campaignId, "Początek", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        Assert.True(campaignResult.Success);
        Assert.True(result.Success);
        Assert.Equal(campaignId, context.Sessions.First().CampaignId);
        Assert.Single(context.Sessions);
    }

    [Fact]
    public void CreateSession_CampaignDoesNotExist_ReturnFail()
    {
        using var context = CreateContext();
        var sessionService = new SessionService(context);

        var result = sessionService.CreateSession(
            9999, "Początek", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        Assert.False(result.Success);
        Assert.Empty(context.Sessions);

    }

    [Fact]
    public void CreateSession_EmptyTitle_ReturnFail()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var result = sessionService.CreateSession(
            campaignId, "", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        Assert.True(campaignResult.Success);
        Assert.False(result.Success);
        Assert.Empty(context.Sessions);
    }

    [Fact]
    public void CreateSession_InvalidDuration_ReturnFail()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var result = sessionService.CreateSession(
            campaignId, "Tytuł", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), -10, SessionStatus.Planned);

        Assert.True(campaignResult.Success);
        Assert.False(result.Success);
        Assert.Empty(context.Sessions);
    }

    [Fact]
    public void CreateSession_InvalidStatus_ReturnFail()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var result = sessionService.CreateSession(
            campaignId, "Tytuł", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 10, (SessionStatus)999);

        Assert.True(campaignResult.Success);
        Assert.False(result.Success);
        Assert.Empty(context.Sessions);
    }

    [Fact]
    public void GetSessions_ReturnAllSessionsForCampaign()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult1 = campaignService.CreateCampaign(
            "Moja 1", "DND", CampaignStatus.Aktywna);

        var campaignId1 = context.Campaigns.First().Id;

        var campaignResult2 = campaignService.CreateCampaign(
            "Moja 2", "DND", CampaignStatus.Aktywna);

        var campaignId2 = context.Campaigns.Last().Id;

        var result1 = sessionService.CreateSession(
            campaignId1, "Początek", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);
        var result2 = sessionService.CreateSession(
            campaignId1, "Początek", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        var result3 = sessionService.CreateSession(
            campaignId2, "Początek", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        var sessions = sessionService.GetSessions(campaignId1);

        Assert.True(campaignResult1.Success);
        Assert.True(campaignResult2.Success);
        Assert.True(result1.Success);
        Assert.True(result2.Success);
        Assert.True(result3.Success);
        Assert.Equal(2, sessions.Count);
        Assert.All(sessions, s => Assert.Equal(campaignId1, s.CampaignId));
    }

    [Fact]
    public void GetSessions_NoSessions_ReturnEmptyList()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var sessions = sessionService.GetSessions(campaignId);

        Assert.True(campaignResult.Success);
        Assert.Empty(sessions);
    }

    [Fact]
    public void DeleteSession_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var result1 = sessionService.CreateSession(
            campaignId, "Początek", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        var sessionId = context.Sessions.First().Id;

        var result2 = sessionService.CreateSession(
            campaignId, "Początek", "jakiś opis", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        OperationResult deletingSessionResult = sessionService.DeleteSession(campaignId,sessionId);

        Assert.True(campaignResult.Success);
        Assert.True(result1.Success);
        Assert.True(result2.Success);
        Assert.True(deletingSessionResult.Success);

        Assert.Single(context.Sessions);
        Assert.DoesNotContain(context.Sessions, s => s.Id == sessionId);
    }

    [Fact]
    public void DeleteSession_SessionDoesNotExist_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        OperationResult deletingSessionResult = sessionService.DeleteSession(campaignId, 999);

        Assert.True(campaignResult.Success);
        Assert.False(deletingSessionResult.Success);
        Assert.Empty(context.Sessions);
    }

    [Fact]
    public void EditSession_ValidData_ReturnSuccess()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var result1 = sessionService.CreateSession(
            campaignId, "Początek", "Pierwsza sesja", new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        var sessionId = context.Sessions.First().Id;

        OperationResult editedSession = sessionService.EditSession(campaignId,sessionId,240,"Mroczne Wrota", "Spotkanie w świątyni", new DateTime(2026, 11, 5, 19, 0, 0), SessionStatus.Completed);

        var updatedSession = context.Sessions.First(s => s.Id == sessionId);

        Assert.True(campaignResult.Success);
        Assert.True(result1.Success);
        Assert.True(editedSession.Success);

        Assert.Equal("Mroczne Wrota", updatedSession.Title);
        Assert.Equal("Spotkanie w świątyni", updatedSession.Description);
        Assert.Equal(240, updatedSession.DurationMinutes);
        Assert.Equal(new DateTime(2026, 11, 5, 19, 0, 0), updatedSession.SessionDate);
        Assert.Equal(SessionStatus.Completed, updatedSession.Status);

    }

    [Fact]
    public void EditSession_SessionDoesNotExist_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        OperationResult editedSession = sessionService.EditSession(
            campaignId, 999, 240, "Mroczne Wrota",
            "Spotkanie w świątyni",
            new DateTime(2026, 11, 5, 19, 0, 0), SessionStatus.Completed);

        Assert.True(campaignResult.Success);
        Assert.False(editedSession.Success);
        Assert.Empty(context.Sessions);
    }

    [Fact]
    public void EditSession_InvalidTitle_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        var campaignResult = campaignService.CreateCampaign(
            "Moja kampania", "DND", CampaignStatus.Aktywna);

        var campaignId = context.Campaigns.First().Id;

        var result1 = sessionService.CreateSession(
            campaignId, "Początek", "Pierwsza sesja",
            new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        var sessionId = context.Sessions.First().Id;

        OperationResult editedSession = sessionService.EditSession(
            campaignId, sessionId, 240, "",
            "Nowy opis",
            new DateTime(2026, 11, 5, 19, 0, 0), SessionStatus.Completed);

        var unchangedSession = context.Sessions.First(s => s.Id == sessionId);

        Assert.True(campaignResult.Success);
        Assert.True(result1.Success);
        Assert.False(editedSession.Success);

        Assert.Equal("Początek", unchangedSession.Title);
        Assert.Equal("Pierwsza sesja", unchangedSession.Description);
        Assert.Equal(180, unchangedSession.DurationMinutes);
        Assert.Equal(SessionStatus.Planned, unchangedSession.Status);
    }

    [Fact]
    public void EditSession_WrongCampaign_ReturnFailure()
    {
        using var context = CreateContext();

        var campaignService = new CampaignService(context);
        var sessionService = new SessionService(context);

        campaignService.CreateCampaign("Kampania 1", "DND", CampaignStatus.Aktywna);
        var campaignId1 = context.Campaigns.First().Id;

        campaignService.CreateCampaign("Kampania 2", "DND", CampaignStatus.Aktywna);
        var campaignId2 = context.Campaigns.Last().Id;

        var result1 = sessionService.CreateSession(
            campaignId1, "Początek", "Pierwsza sesja",
            new DateTime(2026, 11, 3, 18, 0, 0), 180, SessionStatus.Planned);

        var sessionId = context.Sessions.First().Id;

        OperationResult editedSession = sessionService.EditSession(
            campaignId2, sessionId, 240, "Mroczne Wrota",
            "Nowy opis",
            new DateTime(2026, 11, 5, 19, 0, 0), SessionStatus.Completed);

        var unchangedSession = context.Sessions.First(s => s.Id == sessionId);

        Assert.True(result1.Success);
        Assert.False(editedSession.Success);
        Assert.Equal("Początek", unchangedSession.Title);
        Assert.Equal(SessionStatus.Planned, unchangedSession.Status);
    }
}


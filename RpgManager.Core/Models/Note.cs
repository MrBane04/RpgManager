namespace RpgManager.Core.Models;
public class Note
{
    public int Id { get; set; }
    public int CampaignId { get; set; }
    public string Content { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public DateTime UpdatedAt { get; set; }

    public Campaign Campaign { get; set; } = null!;

    public Note()
    {

    }
}

namespace RpgManager.Core.Models;

public class Session
{
    private string _title = string.Empty;
    private int? _durationMinutes = null;

    public DateTime SessionDate { get; set; }
    public DateTime CreatedAt { get; set; }
    public int? DurationMinutes
    {
        get => _durationMinutes;
        set
        {
            if (value.HasValue && value <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(DurationMinutes),"Czas trwania musi być większy od zera.");
            }
            else
            {
                _durationMinutes = value;
            }
        }
    }
    public string? Description { get; set; }
    public SessionStatus Status { get; set; } = SessionStatus.Planned;

    public string Title { get => _title;
        set 
        {
            if(string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentException("Wprowadzono złą wartość we właściwości Title klasy Session");
            }
            else
            {
                _title = value;
            }
        }
    }
    public int Id { get; set; }
    public int CampaignId { get; set; }

    public Campaign Campaign { get; set; } = null!;
}
    
    


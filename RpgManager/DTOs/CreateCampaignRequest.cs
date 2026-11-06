namespace RpgManager.DTOs
{
    public class CreateCampaignRequest
    {
        public required string Name { get; set; }
        public required string System { get; set; }
        public required string Status { get; set; }
    }
}

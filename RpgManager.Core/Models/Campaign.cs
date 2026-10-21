namespace RpgManager.Core.Models
{
    public class Campaign
    {
        public string Name { get; set; }
        public string System {  get; set; }
        public string  Status { get; set; }

        public Campaign (string name, string system, string status)
        {
            this.Name = name;
            this.System = system;
            this.Status = status;
        }
    }
}

namespace RpgManager.Core.Models
{
    public class Campaign
    {
        private string _name = string.Empty;
        private string _system = string.Empty;
        public string Name { get => _name; 
            set 
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Wprowadzono złą wartość we właściwości Name klasy Campaign");
                }
                else
                {
                    _name = value;
                }
            } 
        }
        public string System { get => _system; 
            set 
            {
                if(string.IsNullOrWhiteSpace(value))
                {
                    throw new ArgumentException("Wprowadzono złą wartość we właściwości System klasy Campaign");
                }
                else
                {
                    _system = value;
                }
            } 
        }
        public CampaignStatus  Status { get; set; }
        public int Id { get; set; }

        public Campaign (string name, string system, CampaignStatus status)
        {
            this.Name = name;
            this.System = system;
            this.Status = status;
        }
    }
}

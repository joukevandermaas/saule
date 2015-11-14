namespace Tests.Models
{
    public class Company
    {
        public Company(bool prefill = false)
        {
            if (!prefill) return;

            Id = "456";
            Name = "Awesome, Inc.";
            NumberOfEmployees = 24;
            Location = LocationType.National;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int NumberOfEmployees { get; set; }
        public LocationType Location { get; set; }
    }

    public enum LocationType
    {
        Local,
        National,
        Global
    }
}
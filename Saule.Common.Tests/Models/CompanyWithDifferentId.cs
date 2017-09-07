namespace Tests.Models
{
    public class CompanyWithDifferentId
    {
        public CompanyWithDifferentId(bool prefill = false, string id = "456")
        {
            CompanyId = id;
            if (!prefill) return;

            Name = "Awesome, Inc.";
            NumberOfEmployees = 24;
            Location = LocationType.National;
        }

        public string CompanyId { get; set; }
        public string Name { get; set; }
        public int NumberOfEmployees { get; set; }
        public LocationType Location { get; set; }
    }
}
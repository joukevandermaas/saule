namespace Tests.Helpers
{
    public class Company
    {
        public Company(bool prefill = false)
        {
            if (!prefill) return;

            Id = "456";
            Name = "Awesome, Inc.";
            NumberOfEmployees = 24;
        }

        public string Id { get; set; }
        public string Name { get; set; }
        public int NumberOfEmployees { get; set; }
    }
}
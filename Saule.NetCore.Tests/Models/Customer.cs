namespace Tests.Models
{
    public class Customer
    {
        public Customer(bool prefill = false, string id = "789")
        {
            Id = id;
            if (!prefill) return;

            Name = "Acme PLC";
        }

        public string Id { get; set; }
        public string Name { get; set; }
    }
}

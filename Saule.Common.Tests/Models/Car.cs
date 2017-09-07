namespace Tests.Models
{
    public class Car
    {
        public Car(bool prefill = false, string id = "12")
        {
            Id = id;
            if (!prefill) return;

            Model = "Duster";
        }

        public string Id { get; set; }
        public string Model { get; set; }
    }
}
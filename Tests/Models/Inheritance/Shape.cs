namespace Tests.Models.Inheritance
{
    public abstract class Shape
    {
        protected Shape(bool prefill = false, string id = "123")
        {
            Id = id;
            if (!prefill) return;

            Color = "Purple";
        }

        public string Id { get; set; }
        public string Color { get; set; }
    }
}
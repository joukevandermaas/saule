namespace Tests.Models.Inheritance
{
    public class Circle : Shape
    {
        public Circle(bool prefill = false, string id = "123")
            : base(prefill, id)
        {
            if (!prefill) return;

            Radius = 42;
        }

        
        public decimal Radius { get; set; }
    }
}
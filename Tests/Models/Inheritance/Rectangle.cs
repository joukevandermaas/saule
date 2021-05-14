namespace Tests.Models.Inheritance
{
    public class Rectangle : Shape
    {
        public Rectangle(bool prefill = false, string id = "123")
            : base(prefill, id)
        {
            if (!prefill) return;

            Left = 10;
            Top = 10;
            Width = 100;
            Height = 100;
        }

        
        public int Left { get; set; }
        public int Top { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
    }
}
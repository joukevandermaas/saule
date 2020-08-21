using System.Collections.Generic;

namespace Tests.Models.Inheritance
{
    public class Group
    {
        public Group(bool prefill = false, string id = "123")
        {
            Id = id;
            if (!prefill) return;

            Name = $"Group {id}";
        }
        
        public string Id { get; set; }
        public string Name { get; set; }
        public List<Shape> Shapes { get; set; }
    }
}
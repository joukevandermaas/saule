using Saule;

namespace Tests.Models
{
    internal static class Recursion
    {
        public class Resource : ApiResource
        {
            public Resource()
            {
                BelongsTo<Second>("Model");
            }

            private class Second : ApiResource
            {
                public Second()
                {
                    BelongsTo<Resource>("Model");
                }
            }
        }

        public class FirstModel
        {
            public string Id => "123";
            public SecondModel Model { get; set; }
        }

        public class SecondModel
        {
            public string Id => "456";
            public FirstModel Model { get; set; }
        }
    }
}

using Saule;

namespace Tests.Models
{
    internal static class Recursion
    {
        public class FirstModelResource : ApiResource
        {
            public FirstModelResource()
            {
                BelongsTo<SecondModelResource>("Child");
            }
        }

        public class SecondModelResource : ApiResource
        {
            public SecondModelResource()
            {
                BelongsTo<FirstModelResource>("Parent");
                BelongsTo<ThirdModelResource>("Child");
            }
        }

        public class ThirdModelResource : ApiResource
        {
            public ThirdModelResource()
            {
                BelongsTo<SecondModelResource>("Parent");
                BelongsTo<FourthModelResource>("Child");
            }
        }

        public class FourthModelResource : ApiResource
        {
            public FourthModelResource()
            {
                BelongsTo<ThirdModelResource>("Parent");
            }
        }

        public class FirstModel
        {
            public string Id = "1";
            public SecondModel Child { get; set; }
        }

        public class SecondModel
        {
            public string Id = "2";
            public FirstModel Parent { get; set; }
            public ThirdModel Child { get; set; }
        }

        public class ThirdModel
        {
            public string Id = "3";
            public SecondModel Parent { get; set; }
            public FourthModel Child { get; set; }
        }

        public class FourthModel
        {
            public string Id = "4";
            public ThirdModel Parent { get; set; }
        }
    }
}

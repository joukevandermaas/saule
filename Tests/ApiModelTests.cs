using Saule;
using Xunit;

namespace Tests
{
    public class ApiModelTests
    {
        private class TestApiModel : ApiModel
        { }
        private class TestApiModel2 : ApiModel
        { }

        [Fact(DisplayName = "Model name defaults to class name")]
        public void UsesClassName()
        {
            var model = new TestApiModel();

            Assert.Equal("TestApi", model.ModelType);

            var model2 = new TestApiModel2();

            Assert.Equal("TestApiModel2", model2.ModelType);
        }
    }
}

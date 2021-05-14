using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Tests.Models.Inheritance;

namespace Tests.Controllers
{
    [RoutePrefix("api")]
    public class ShapeController : ApiController
    {
        [HttpGet]
        [Route("shapes")]
        [AllowsQuery]
        [ReturnsResource(typeof(ShapeResource))]
        public IEnumerable<Shape> GetAllShapes()
        {
            return new Shape[]
            {
                new Circle(true, "1"),
                new Rectangle(true, "2")
                {
                    Color = "Green"
                },
                new Circle(true, "3"),
            };
        }
        
        [HttpGet]
        [Route("shape/{id}")]
        [ReturnsResource(typeof(ShapeResource))]
        public Shape GetShape(string id)
        {
            return GetAllShapes().FirstOrDefault(s => s.Id == id);
        }

        [HttpGet]
        [Route("groups")]
        [AllowsQuery]
        [DisableDefaultIncluded]
        [ReturnsResource(typeof(GroupResource))]
        public IEnumerable<Group> GetGroup()
        {
            return new[]
            {
                new Group(true, "1")
                {
                    Shapes = GetAllShapes().ToList()
                }
            };
        }
    }
}
using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Tests.Models.Inheritance;

namespace Tests.Controllers
{
    [ReturnsResource(typeof(ShapeResource))]
    [RoutePrefix("api")]
    public class ShapeController : ApiController
    {
        [HttpGet]
        [Route("shapes")]
        public IEnumerable<Shape> GetAllShapes()
        {
            return new Shape[]
            {
                new Circle(true, "1"),
                new Rectangle(true, "2"),
                new Circle(true, "3"),
            };
        }
        
        [HttpGet]
        [Route("shape/{id}")]
        public Shape GetShape(string id)
        {
            return GetAllShapes().FirstOrDefault(s => s.Id == id);
        }
    }
}
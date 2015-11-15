using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;

namespace Tests.Controllers
{
    [ReturnsResource(typeof(CompanyResource))]
    public class CompaniesController : ApiController
    {
        [HttpGet]
        [Route("companies/{id}")]
        public Company GetCompany(string id)
        {
            return Get.Company(id);
        }

        [HttpGet]
        [Paginated(PerPage = 12)]
        [Route("companies")]
        public IEnumerable<Company> GetCompanies()
        {
            return Get.Companies(100);
        }
    }
}

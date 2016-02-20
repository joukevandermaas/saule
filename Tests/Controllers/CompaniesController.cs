using System.Collections.Generic;
using System.Web.Http;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;

namespace Tests.Controllers
{
    [RoutePrefix("api")]
    public class CompaniesController : ApiController
    {
        [HttpGet]
        [Route("companies/{id}")]
        [ReturnsResource(typeof(CompanyResource))]
        public Company GetCompany(string id)
        {
            var company = Get.Company(id);
            company.Location = LocationType.National;

            return company;
        }

        // note: no ReturnsResource!
        [HttpDelete]
        [Route("companies/{id}")]
        public void DeleteCompany(string id)
        {
            
        }

        [HttpGet]
        [Paginated(PerPage = 12)]
        [Route("companies")]
        [ReturnsResource(typeof(CompanyResource))]
        public IEnumerable<Company> GetCompanies()
        {
            return Get.Companies(100);
        }
    }
}

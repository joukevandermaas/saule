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

        [HttpGet]
        [Paginated(PerPage = 12)]
        [Route("companies/querypagesize")]
        [ReturnsResource(typeof(CompanyResource))]
        public IEnumerable<Company> GetCompaniesQueryPageSize()
        {
            return Get.Companies(100);
        }

        [HttpGet]
        [Paginated(PerPage = 12, PageSizeLimit = 50)]
        [Route("companies/querypagesizelimit50")]
        [ReturnsResource(typeof(CompanyResource))]
        public IEnumerable<Company> GetCompaniesQueryPageSizeLimit50()
        {
            return Get.Companies(100);
        }

	    [HttpGet]
	    [Paginated(PerPage = 1, PageSizeLimit = 1)]
	    [Route("companies/querypagesizelimit1")]
	    [ReturnsResource(typeof(CompanyResource))]
	    public IEnumerable<Company> GetCompaniesQueryPageSizeLimit1()
	    {
		    return Get.Companies(20);
	    }
	}
}

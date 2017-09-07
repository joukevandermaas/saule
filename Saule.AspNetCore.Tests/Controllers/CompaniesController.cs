using System.Collections.Generic;
using Saule.Http;
using Tests.Helpers;
using Tests.Models;
using Microsoft.AspNetCore.Mvc;

namespace Tests.Controllers
{
    [Route("api")]
    public class CompaniesController : Controller
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
        public IActionResult DeleteCompany(string id)
        {
            // Net Core will return an Ok result when a controller actions returns void
            // changed to IActionResult and NoContent();

            return NoContent();
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

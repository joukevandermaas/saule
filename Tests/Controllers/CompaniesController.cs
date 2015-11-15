using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Tests.Models;

namespace Tests.Controllers
{
    [ReturnsResource(typeof(CompanyResource))]
    public class CompaniesController : ApiController
    {
        [HttpGet]
        public Company GetCompany(string id)
        {
            return new Company(prefill: true);
        }

        [HttpGet]
        [Paginated]
        public IEnumerable<Company> GetCompanies()
        {
            return GetEnumerable().Take(100);
        }

        private IEnumerable<Company> GetEnumerable()
        {
            var i = 0;
            while (true)
            {
                yield return new Company(prefill: true, id: i++.ToString());
            }
        }
    }
}

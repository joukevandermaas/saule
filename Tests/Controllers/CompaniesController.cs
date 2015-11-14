using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
    }
}

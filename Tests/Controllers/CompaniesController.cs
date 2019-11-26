﻿using System.Collections.Generic;
using System.Linq;
using System.Web.Http;
using Saule.Http;
using Saule.Queries.Pagination;
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
        [JsonApi]
        [Paginated(PerPage = 12)]
        [Route("v2/companies")]
        [ReturnsResource(typeof(CompanyResource))]
        public IEnumerable<Company> GetCompaniesV2()
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

        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20)]
        [Route("companies/paged-result")]
        [ReturnsResource(typeof(CompanyResource))]
        public PagedResult<Company> GetCompaniesWithPaging()
        {
            return new PagedResult<Company>()
            {
                TotalResultsCount = 100,
                Data = Get.Companies(100).ToList()
            };
        }

        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20)]
        [Route("companies/paged-result-custom")]
        [ReturnsResource(typeof(CompanyResource))]
        public PagedResult<Company> GetCompaniesWithCustomPaging()
        {
            return new CustomPagedResult<Company>()
            {
                TotalResultsCount = 100,
                Data = Get.Companies(100).ToList()
            };
        }

        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20, FirstPageNumber = 1)]
        [Route("companies/paged-result-first-page")]
        [ReturnsResource(typeof(CompanyResource))]
        public PagedResult<Company> GetCompaniesWithPagingAndFirstPage()
        {
            return new PagedResult<Company>()
            {
                TotalResultsCount = 100,
                Data = Get.Companies(100).ToList()
            };
        }

        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20, FirstPageNumber = 1)]
        [Route("companies/paged-result-empty-first-page")]
        [ReturnsResource(typeof(CompanyResource))]
        public PagedResult<Company> GetEmpyCompaniesFirstPage()
        {
            return new PagedResult<Company>()
            {
                TotalResultsCount = 0,
                Data = new List<Company>()
            };
        }

        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20)]
        [Route("companies/paged-result-empty")]
        [ReturnsResource(typeof(CompanyResource))]
        public PagedResult<Company> GetEmpyCompanies()
        {
            return new PagedResult<Company>()
            {
                TotalResultsCount = 0,
                Data = new List<Company>()
            };
        }

        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20)]
        [Route("companies/paged-result-10items")]
        [ReturnsResource(typeof(CompanyResource))]
        public PagedResult<Company> GetCompaniesWith10Items()
        {
            return new PagedResult<Company>()
            {
                TotalResultsCount = 10,
                Data = Get.Companies(10).ToList()
            };
        }


        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20, FirstPageNumber = 1)]
        [Route("companies/paged-result-10items-first-page")]
        [ReturnsResource(typeof(CompanyResource))]
        public PagedResult<Company> GetCompaniesWith10ItemsAndFirstPage()
        {
            return new PagedResult<Company>()
            {
                TotalResultsCount = 10,
                Data = Get.Companies(10).ToList()
            };
        }

        [HttpGet]
        [Paginated(PerPage = 20, PageSizeLimit = 20)]
        [Route("companies/paged-result-queryable")]
        [ReturnsResource(typeof(CompanyResource))]
        public IQueryable<Company> GetCompaniesWithPagingAndQueryable()
        {
            return Get.Companies(100).AsQueryable();
        }
    }
}

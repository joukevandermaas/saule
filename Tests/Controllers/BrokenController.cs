﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using Saule.Http;
using Saule.Queries;
using Tests.Helpers;
using Tests.Models;

namespace Tests.Controllers
{
    public class BrokenController : ApiController
    {
        [HttpGet]
        [Route("api/broken/{id}")]
        public Person GetPerson(string id)
        {
            return Get.Person(id);
        }

        [HttpGet]
        [ReturnsResource(typeof(PersonResource))]
        [Route("api/broken")]
        [Authorize]
        public IQueryable<Person> GetPeople()
        {
            return Get.People(20).AsQueryable();
        }

        [HttpGet]
        [Route("api/broken/errors")]
        [ReturnsResource(typeof(PersonResource))]
        public HttpResponseMessage TwoErrors()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, new List<HttpError>()
            {
                new HttpError("Error 1")
                {
                    ExceptionType = "Type 1"
                },
                new HttpError("Error 2")
                {
                    ExceptionType = "Type 2"
                }
            });
        }

        [HttpGet]
        [Route("api/broken/errorsNoResource")]
        public HttpResponseMessage TwoErrorsNoResource()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest, new List<HttpError>()
            {
                new HttpError("Error 1")
                {
                    ExceptionType = "Type 1"
                },
                new HttpError("Error 2")
                {
                    ExceptionType = "Type 2"
                }
            });
        }

        [HttpGet]
        [Route("api/broken/error")]
        [ReturnsResource(typeof(PersonResource))]
        public HttpResponseMessage OneError()
        {
            return Request.CreateResponse(HttpStatusCode.BadRequest,
                new HttpError("Error 1")
                {
                    ExceptionType = "Type 1"
                });
        }

        [HttpGet]
        [Route("api/broken/exception")]
        [ReturnsResource(typeof(PersonResource))]
        public HttpResponseMessage Exception()
        {
            throw new InvalidOperationException("Test exception");
        }

        [HttpGet]
        [HandlesQuery]
        [DisableDefaultIncluded]
        [Route("api/broken/manual/disabledefault")]
        public IQueryable<Person> ManualQueryAndDisableDefault(QueryContext context)
        {
            return Get.People(1).AsQueryable();
        }

        [HttpGet]
        [HandlesQuery]
        [AllowsQuery]
        [Route("api/broken/manual/allowsquery")]
        public IQueryable<Person> ManualQueryAndAllowsQuery(QueryContext context)
        {
            return Get.People(1).AsQueryable();
        }

    }
}

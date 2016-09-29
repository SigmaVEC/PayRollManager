using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PayRollManager.Controllers {
    public class ListCompaniesController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/ListCompanies
        [ResponseType(typeof(Company_Info))]
        [HttpGet]
        public IHttpActionResult List() {
            return Ok(db.Company_Info);
        }
    }
}

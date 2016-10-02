using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayRollManager.Controllers {
    public class ListCompaniesController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/ListCompanies
        [HttpGet]
        public IHttpActionResult List() {
            return Ok(new Message {
                data = db.Company_Info.ToArray(),
                message = "Success"
            });
        }
    }
}

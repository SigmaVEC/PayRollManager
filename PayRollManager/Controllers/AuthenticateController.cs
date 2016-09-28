using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using PayRollManager.Models;

namespace PayRollManager.Controllers {
    public class AuthenticateController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // POST: api/Authenticate
        [ResponseType(typeof(Company_Details))]
        [HttpGet]
        public IHttpActionResult Auth(String companyName, String id, String password) {
            var res = db.Company_Details.FirstOrDefaultAsync((p) => (p.CompanyName == companyName && p.EmployeeId == id && p.Password == password));
            return Ok(res.Result);
        }
    }
}
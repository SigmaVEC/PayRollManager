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

        // GET: api/Authenticate
        [ResponseType(typeof(Employee_Info))]
        [HttpGet]
        public IHttpActionResult Auth(int companyId, String employeeId, String password) {
            var company = db.Company_Info.FirstOrDefaultAsync((p) => (p.CompanyId == companyId));

            if(company.Result != null) {
                var employee = db.Employee_Info.FirstOrDefaultAsync((p) => (p.EmployeeId == employeeId && p.Password == password));
                return Ok(employee.Result);
            } else {
                return Ok(company.Result);
            }
        }
    }
}
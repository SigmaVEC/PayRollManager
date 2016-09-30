using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayRollManager.Controllers {
    public class EmployeeViewController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/EmployeeView
        [HttpGet]
        public IHttpActionResult View(String token) {
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token));

            if(session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));
                var reply = new EmployeeViewModel {
                    id = employee.EmployeeId,
                    name = employee.EmployeeName,
                    doj = employee.DOJ,
                    salary = db.Employee_Salary.Where((p) => (p.CompanyId == employee.CompanyId && p.EmployeeId == employee.EmployeeId)).ToArray()
                };

                return Ok(reply);
            } else {
                return Ok(session);
            }
        }
    }
}

using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayRollManager.Controllers {
    public class RemoveCompanyController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/RemoveEmployee
        [HttpGet]
        public IHttpActionResult RemoveCompany(String token, int companyId) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    var company = db.Company_Info.FirstOrDefault((p) => (p.CompanyId == companyId));
                    db.Company_Info.Remove(company);

                    var companyEmployees = db.Employee_Info.Where((p) => (p.CompanyId == companyId));
                    foreach(var i in companyEmployees) {
                        db.Employee_Info.Remove(i);
                    }

                    var companySalaries = db.Employee_Salary.Where((p) => (p.CompanyId == companyId));
                    foreach (var i in companySalaries) {
                        db.Employee_Salary.Remove(i);
                    }

                    var companySessions = db.Session_Tokens.Where((p) => (p.CompanyId == companyId));
                    foreach (var i in companySessions) {
                        db.Session_Tokens.Remove(i);
                    }
                    db.SaveChangesAsync();

                    return Ok(new Message {
                        data = null,
                        message = "Success"
                    });
                } else {
                    return Ok(new Message {
                        data = null,
                        message = "You do not have permission to perform this operation"
                    });
                }
            } else {
                return Ok(new Message {
                    data = null,
                    message = "Session Token is invalid"
                });
            }
        }
    }
}

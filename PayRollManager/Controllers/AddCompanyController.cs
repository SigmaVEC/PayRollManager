using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayRollManager.Controllers {
    public class AddCompanyController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/AddCompany
        [HttpGet]
        public IHttpActionResult AddCompany(String token, String companyName) {
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token));

            if (session != null) {
                if(companyName != null) {
                    var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));

                    if (employee.IsAdmin.Equals("y")) {
                        var companyId = db.Company_Info.Max(p => p.CompanyId) + 1;
                        var company = new Company_Info {
                            CompanyId = companyId,
                            CompanyName = companyName
                        };
                        db.Company_Info.Add(company);
                        db.SaveChanges();

                        return Ok(db.Company_Info.FirstOrDefault((p) => (p.CompanyId == company.CompanyId)));
                    } else {
                        return Ok(new ErrorMessage { message = "You do not have permission to perform this operation" });
                    }
                } else {
                    return Ok(new ErrorMessage { message = "Company Name is empty" });
                }
            } else {
                return Ok(new ErrorMessage { message = "Session Token is invalid" });
            }
        }
    }
}
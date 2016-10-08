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
    public class AddCompanyController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/AddCompany
        [HttpGet]
        public IHttpActionResult AddCompany(String token, String companyName) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                if(companyName != null) {
                    var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                    if (employee != null) {
                        var companyId = db.Company_Info.Max(p => p.CompanyId) + 1;
                        var company = new Company_Info {
                            CompanyId = companyId,
                            CompanyName = companyName
                        };
                        db.Company_Info.Add(company);
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
                        message = "Company Name is empty"
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
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayRollManager.Models;
using System.Security.Cryptography;
using System.Web;

namespace PayRollManager.Controllers {
    public class AuthenticateController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/Authenticate
        [HttpGet]
        public IHttpActionResult Auth(int companyId, String employeeId, String password) {
            var company = db.Company_Info.FirstOrDefault((p) => (p.CompanyId == companyId));

            if(company != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.EmployeeId == employeeId && p.Password == password && p.DOL == null));

                if(employee != null) {
                    var prevToken = db.Session_Tokens.FirstOrDefault((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId));

                    if (prevToken != null) {
                        db.Session_Tokens.Remove(prevToken);
                        db.SaveChanges();
                    }

                    var rng = new RNGCryptoServiceProvider();
                    var randBytes = new byte[50];
                    rng.GetBytes(randBytes);
                    var token = new Session_Tokens {
                        SessionToken = HttpServerUtility.UrlTokenEncode(randBytes),
                        Timestamp = DateTime.Now,                        
                        CompanyId = companyId,
                        EmployeeId = employeeId
                    };
                    db.Session_Tokens.Add(token);
                    db.SaveChanges();

                    var session = db.Session_Tokens.FirstOrDefault((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId));
                    return Ok(new Message {
                        data = session,
                        message = "Success"
                    });
                } else {
                    return Ok(new Message {
                        data = null,
                        message = "Employee ID and/or Password invalid"
                    });
                }
            } else {
                return Ok(new Message {
                    data = null,
                    message = "Company ID invalid"
                });
            }
        }
    }
}
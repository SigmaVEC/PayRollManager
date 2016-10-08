using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace PayRollManager.Controllers {
    public class RemoveEmployeeController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/RemoveEmployee
        [HttpGet]
        public IHttpActionResult RemoveEmployee(String token, String employeeJson) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    try {
                        var leftEmployee = new JavaScriptSerializer().Deserialize<EmployeeRemoveModel>(employeeJson);
                        var dbEmployee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == leftEmployee.companyId && p.EmployeeId == leftEmployee.employeeId));

                        if (dbEmployee != null) {
                            dbEmployee.DOL = leftEmployee.dol;
                            var employeeSession = db.Session_Tokens.FirstOrDefault((p) => (p.EmployeeId == dbEmployee.EmployeeId && p.CompanyId == dbEmployee.CompanyId));

                            if(employeeSession != null) {
                                db.Session_Tokens.Remove(employeeSession);
                            }
                            db.SaveChangesAsync();
                            return Ok(new Message {
                                data = null,
                                message = "Success"
                            });
                        } else {
                            return Ok(new Message {
                                data = null,
                                message = "Employee does not exist"
                            });
                        }
                    } catch (System.ArgumentException) {
                        return Ok(new Message {
                            data = null,
                            message = "JSON format is invalid"
                        });
                    }
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

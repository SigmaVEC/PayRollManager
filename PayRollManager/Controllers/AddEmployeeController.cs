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
    public class AddEmployeeController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/AddEmployee
        [HttpGet]
        public IHttpActionResult AddEmployee(String token, String employeeJson) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    try {
                        var newEmployee = new JavaScriptSerializer().Deserialize<EmployeeDataModel>(employeeJson);
                        var dbEmployee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == newEmployee.companyId && p.EmployeeId == newEmployee.employeeId));

                        if (dbEmployee == null) {
                            var e = new Employee_Info {
                                CompanyId = newEmployee.companyId,
                                EmployeeId = newEmployee.employeeId,
                                EmployeeName = newEmployee.name,
                                DOJ = newEmployee.date,
                                DOL = null,
                                IsAdmin = newEmployee.isAdmin,
                                Password = newEmployee.password
                            };
                            db.Employee_Info.Add(e);

                            for (int i = 0; i < newEmployee.salary.Length; i++) {
                                if((newEmployee.salary[i].type == "#") || (newEmployee.salary[i].type == "%" && newEmployee.salary[i].value <= 100)) {
                                    var s = new Employee_Salary {
                                        CompanyId = newEmployee.companyId,
                                        EmployeeId = newEmployee.employeeId,
                                        AdjustmentName = newEmployee.salary[i].name,
                                        AdjustmentType = newEmployee.salary[i].type,
                                        AdjustmentValue = newEmployee.salary[i].value
                                    };
                                    db.Employee_Salary.Add(s);
                                } else {
                                    return Ok(new Message {
                                        data = null,
                                        message = "Salary data model contains invalid data"
                                    });
                                }
                            }
                            db.SaveChangesAsync();

                            return Ok(new Message {
                                data = null,
                                message = "Success"
                            });
                        } else {
                            return Ok(new Message {
                                data = null,
                                message = "Employee already exists"
                            });
                        }
                    } catch(System.ArgumentException) {
                        return Ok(new Message {
                            data = null,
                            message = "JSON format is invalid"
                        });
                    }
                } else {
                    return Ok(new Message {
                        data= null,
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

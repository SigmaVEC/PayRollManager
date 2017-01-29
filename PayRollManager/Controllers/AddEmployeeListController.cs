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
    public class AddEmployeeListController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/AddEmployee
        [HttpGet]
        public IHttpActionResult AddEmployeeList(String token, String employeeJson) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    try {
                        var employees = new JavaScriptSerializer().Deserialize<EmployeeListDataModel>(employeeJson);

                        for (int i = 0; i < employees.employee.Length; i++) {
                            var newEmployee = employees.employee[i];
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

                                if (newEmployee.salary.FirstOrDefault((p) => (p.name == "Basic" && p.type == "#" && p.value > 0)) != null) {
                                    for (int j = 0; j < newEmployee.salary.Length; j++) {
                                        if ((newEmployee.salary[j].type == "#") || (newEmployee.salary[j].type == "%" && newEmployee.salary[j].value <= 100)) {
                                            var s = new Employee_Salary {
                                                CompanyId = newEmployee.companyId,
                                                EmployeeId = newEmployee.employeeId,
                                                AdjustmentName = newEmployee.salary[j].name,
                                                AdjustmentType = newEmployee.salary[j].type,
                                                AdjustmentValue = newEmployee.salary[j].value
                                            };
                                            db.Employee_Salary.Add(s);
                                        } else {
                                            return Ok(new Message {
                                                data = null,
                                                message = "Salary data model contains invalid data"
                                            });
                                        }
                                    }

                                    for (int j = 0; j < newEmployee.personal.Length; j++) {
                                        if ((newEmployee.personal[j].name.Length != 0) && (newEmployee.personal[j].value.Length != 0)) {
                                            var p = new Personal_Details {
                                                CompanyId = newEmployee.companyId,
                                                EmployeeId = newEmployee.employeeId,
                                                Name = newEmployee.personal[j].name,
                                                Value = newEmployee.personal[j].value
                                            };
                                            db.Personal_Details.Add(p);
                                        } else {
                                            return Ok(new Message {
                                                data = null,
                                                message = "Personal data model contains invalid data"
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
                                        message = "Basic pay not found"
                                    });
                                }
                            } else {
                                return Ok(new Message {
                                    data = null,
                                    message = "Employee already exists"
                                });
                            }
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

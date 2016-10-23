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
    public class UpdateEmployeeController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/UpdateEmployee
        [HttpGet]
        public IHttpActionResult UpdateEmployee(String token, String employeeJson) {

            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    try {
                        var updateEmployee = new JavaScriptSerializer().Deserialize<EmployeeDataModel>(employeeJson);
                        var dbEmployee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == updateEmployee.companyId && p.EmployeeId == updateEmployee.employeeId));

                        if (dbEmployee != null) {
                            var dbEmployeeSalary = db.Employee_Salary.Where((p) => (p.CompanyId == updateEmployee.companyId && p.EmployeeId == updateEmployee.employeeId));
                            var dbEmployeePersonal = db.Personal_Details.Where((p) => (p.CompanyId == updateEmployee.companyId && p.EmployeeId == updateEmployee.employeeId));
                            var dbEmployeeSession = db.Session_Tokens.FirstOrDefault((p) => (p.CompanyId == updateEmployee.companyId && p.EmployeeId == updateEmployee.employeeId));

                            dbEmployee.EmployeeName = updateEmployee.name;
                            dbEmployee.DOJ = updateEmployee.date;
                            dbEmployee.IsAdmin = updateEmployee.isAdmin;
                            dbEmployee.Password = updateEmployee.password;

                            db.Employee_Salary.RemoveRange(dbEmployeeSalary);
                            db.Personal_Details.RemoveRange(dbEmployeePersonal);
                            db.Session_Tokens.Remove(dbEmployeeSession);

                            for (int i = 0; i < updateEmployee.salary.Length; i++) {
                                if ((updateEmployee.salary[i].type == "#") || (updateEmployee.salary[i].type == "%" && updateEmployee.salary[i].value <= 100)) {
                                    var s = new Employee_Salary {
                                        CompanyId = updateEmployee.companyId,
                                        EmployeeId = updateEmployee.employeeId,
                                        AdjustmentName = updateEmployee.salary[i].name,
                                        AdjustmentType = updateEmployee.salary[i].type,
                                        AdjustmentValue = updateEmployee.salary[i].value
                                    };
                                    db.Employee_Salary.Add(s);
                                } else {
                                    return Ok(new Message {
                                        data = null,
                                        message = "Salary data model contains invalid data"
                                    });
                                }
                            }

                            for (int i = 0; i < updateEmployee.personal.Length; i++) {
                                if ((updateEmployee.personal[i].name.Length != 0) && (updateEmployee.personal[i].value.Length != 0)) {
                                    var p = new Personal_Details {
                                        CompanyId = updateEmployee.companyId,
                                        EmployeeId = updateEmployee.employeeId,
                                        Name = updateEmployee.personal[i].name,
                                        Value = updateEmployee.personal[i].value
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
                                message = "Employee deos not exist"
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

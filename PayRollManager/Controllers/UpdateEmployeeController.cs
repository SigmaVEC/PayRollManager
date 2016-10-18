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
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));

                if (employee != null && employee.IsAdmin == "y") {
                    try {
                        var updateEmployee = new JavaScriptSerializer().Deserialize<EmployeeDataModel>(employeeJson);
                        var dbEmployee = db.Employee_Info.FirstOrDefault((p) => (p.EmployeeId == updateEmployee.employeeId));

                        if (dbEmployee != null) {
                            dbEmployee.CompanyId = updateEmployee.companyId;
                            dbEmployee.EmployeeName = updateEmployee.name;
                            dbEmployee.DOJ = updateEmployee.date;
                            dbEmployee.IsAdmin = updateEmployee.isAdmin;
                            dbEmployee.Password = updateEmployee.password;

                            var updateEmployeeSal = db.Employee_Salary.FirstOrDefault((p) => (p.EmployeeId == updateEmployee.employeeId));
                            for (int i = 0; i < updateEmployee.salary.Length; i++) {
                                if ((updateEmployee.salary[i].type == "#") || (updateEmployee.salary[i].type == "%" && updateEmployee.salary[i].value <= 100)) {
                                    updateEmployeeSal.AdjustmentName = updateEmployee.salary[i].name;
                                    updateEmployeeSal.AdjustmentType = updateEmployee.salary[i].type;
                                    updateEmployeeSal.AdjustmentValue = updateEmployee.salary[i].value;
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
                                message = "Employee does not exist"
                            });
                        }
                    } catch (System.ArgumentException) {
                        return Ok(new Message {
                            data = null,
                            message = "JSON format is invalid"
                        });
                    }
                } else if (employee != null && employee.IsAdmin == "n") {
                    try {
                        var updateEmployee = new JavaScriptSerializer().Deserialize<EmployeeDataModel>(employeeJson);
                        var dbEmployee = db.Employee_Info.FirstOrDefault((p) => (p.EmployeeId == updateEmployee.employeeId));

                        if (dbEmployee != null) {
                            dbEmployee.Password = updateEmployee.password;
                            dbEmployee.EmployeeName = updateEmployee.name;
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
                        message = "Employee does not exist"
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

using PayRollManager.Models;
using System;
using System.Collections.Generic;
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
        public IHttpActionResult AddCompany(String token, String newEmployeeJson) {
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));

                if (employee.IsAdmin.Equals("y")) {
                    try {
                        var newEmployee = new JavaScriptSerializer().Deserialize<EmployeeDataModel>(newEmployeeJson);
                        var e = new Employee_Info {
                            CompanyId = newEmployee.companyId,
                            EmployeeId = newEmployee.employeeId,
                            EmployeeName = newEmployee.name,
                            DOJ = newEmployee.doj,
                            DOL = null,
                            IsAdmin = newEmployee.isAdmin,
                            Password = newEmployee.password
                        };
                        db.Employee_Info.Add(e);
                        db.SaveChanges();

                        foreach (var i in newEmployee.salary) {
                            var s = new Employee_Salary {
                                CompanyId = newEmployee.companyId,
                                EmployeeId = newEmployee.employeeId,
                                AdjustmentName = i.name,
                                AdjustmentType = i.type,
                                AdjustmentValue = i.value
                            };
                            db.Employee_Salary.Add(s);
                            db.SaveChanges();
                        }

                        return Ok(newEmployee);
                    } catch(System.ArgumentException) {
                        return Ok(new ErrorMessage { message = "JSON format is invalid" });
                    }
                } else {
                    return Ok(new ErrorMessage { message = "You do not have permission to perform this operation" });
                }
            } else {
                return Ok(new ErrorMessage { message = "Session Token is invalid" });
            }
        }
    }
}

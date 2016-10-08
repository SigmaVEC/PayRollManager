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
    public class EmployeeViewController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/EmployeeView
        [HttpGet]
        public IHttpActionResult View(String token) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));
                var s = db.Employee_Salary.Where((p) => (p.CompanyId == employee.CompanyId && p.EmployeeId == employee.EmployeeId)).ToArray();
                var salaryData = new List<SalaryDataModel>();

                for (int i = 0; i < s.Length; i++) {
                    salaryData.Add(new SalaryDataModel {
                        name = s[i].AdjustmentName,
                        type = s[i].AdjustmentType,
                        value = s[i].AdjustmentValue
                    });
                }

                var reply = new EmployeeViewModel {
                    id = employee.EmployeeId,
                    name = employee.EmployeeName,
                    doj = employee.DOJ,
                    salary = salaryData.ToArray()
                };

                return Ok(new Message {
                    data = reply,
                    message = "Success"
                });
            } else {
                return Ok(new Message {
                    data = null,
                    message = "Session Token is invalid"
                });
            }
        }

        // GET: api/EmployeeView
        [HttpGet]
        public IHttpActionResult View(String token, int companyId) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var admin = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (admin != null) {
                    var employees = db.Employee_Info.Where((p) => (p.CompanyId == companyId)).ToArray();
                    var employeeData = new List<EmployeeViewModel>();

                    if(employees.Length != 0) {
                        for (int i = 0; i < employees.Length; i++) {
                            var employee = employees[i];
                            var s = db.Employee_Salary.Where((p) => (p.CompanyId == employee.CompanyId && p.EmployeeId == employee.EmployeeId)).ToArray();
                            var salaryData = new List<SalaryDataModel>();

                            for (int j = 0; j < s.Length; j++) {
                                salaryData.Add(new SalaryDataModel {
                                    name = s[j].AdjustmentName,
                                    type = s[j].AdjustmentType,
                                    value = s[j].AdjustmentValue
                                });
                            }

                            employeeData.Add(new EmployeeViewModel {
                                id = employee.EmployeeId,
                                name = employee.EmployeeName,
                                doj = employee.DOJ,
                                salary = salaryData.ToArray()
                            });
                        }

                        return Ok(new Message {
                            data = employeeData.ToArray(),
                            message = "Success"
                        });
                    } else {
                        return Ok(new Message {
                            data = null,
                            message = "Company not found"
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

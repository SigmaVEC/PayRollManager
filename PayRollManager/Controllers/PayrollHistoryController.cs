using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayRollManager.Models;
using System.Configuration;
using System.Data.Entity;

namespace PayRollManager.Controllers {
    public class PayrollHistoryController : ApiController {
        PayRollManagerEntities db = new PayRollManagerEntities();

        //GET: /api/PayrollHistory
        [HttpGet]
        public IHttpActionResult EmployeePayroll(String token, String month) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));

                if (employee != null) {
                    var date = DateTime.Parse(month);
                    var history = db.Payroll_History.Where((p) => (p.CompanyId == employee.CompanyId && p.EmployeeId == employee.EmployeeId && p.Month.Month == date.Month && p.Month.Year == date.Year)).ToArray();

                    if(history.Length != 0) {
                        var salaryData = new List<SalaryDataModel>();

                        for(int i = 0; i < history.Length; i++) {
                            salaryData.Add(new SalaryDataModel {
                                name = history[i].AdjustmentName,
                                type = history[i].AdjustmentType,
                                value = history[i].AdjustmentValue
                            });
                        }

                        var employeeData = new EmployeeViewModel {
                            id = employee.EmployeeId,
                            name = employee.EmployeeName,
                            doj = employee.DOJ,
                            salary = salaryData.ToArray()
                        };

                        return Ok(new Message {
                            data = employeeData,
                            message = "Success"
                        });
                    } else {
                        return Ok(new Message {
                            data = null,
                            message = "No Payroll exists for specified month"
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

        //GET: /api/PayrollHistory
        [HttpGet]
        public IHttpActionResult CompanyPayroll(String token, int companyId, String month) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    var date = DateTime.Parse(month);
                    var employeeList = db.Payroll_History.Where((p) => (p.CompanyId == companyId && p.Month.Month == date.Month && p.Month.Year == date.Year)).Select((p) => (p.EmployeeId)).Distinct().ToArray();
                    if (employeeList.Length != 0) {
                        var employeeData = new List<EmployeeViewModel>();
                        
                        for (int i = 0; i < employeeList.Length; i++) {
                            var salaryData = new List<SalaryDataModel>();
                            var employeeId = employeeList[i];
                            var employeeInfo = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId));
                            var history = db.Payroll_History.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId && p.Month.Month == date.Month && p.Month.Year == date.Year)).ToArray();
                            
                            for (int j = 0; j < history.Length; j++) {
                                salaryData.Add(new SalaryDataModel {
                                    name = history[j].AdjustmentName,
                                    type = history[j].AdjustmentType,
                                    value = history[j].AdjustmentValue
                                });
                            }

                            employeeData.Add(new EmployeeViewModel {
                                id = employeeInfo.EmployeeId,
                                name = employeeInfo.EmployeeName,
                                doj = employeeInfo.DOJ,
                                salary = salaryData.ToArray()
                            });
                        }

                        return Ok(new Message {
                            data = employeeData,
                            message = "Success"
                        });
                    } else {
                        return Ok(new Message {
                            data = null,
                            message = "No Payroll exists for specified month"
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

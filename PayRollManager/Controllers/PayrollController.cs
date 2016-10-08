using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace PayRollManager.Controllers {
    public class PayrollController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/AddEmployee
        [HttpGet]
        public IHttpActionResult AddEmployee(String token, String attendanceJson) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    try {
                        var attendanceInput = new JavaScriptSerializer().Deserialize<AttendanceInputModel>(attendanceJson);
                        var employeeData = new List<EmployeeViewModel>();
                        for(int i = 0; i < attendanceInput.attendance.Length; i++) {
                            var empAttendance = attendanceInput.attendance[i];
                            var employeeInfo = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == empAttendance.companyId && p.EmployeeId == empAttendance.employeeId));

                            if(employeeInfo != null) {
                                var s = db.Employee_Salary.Where((p) => (p.CompanyId == employeeInfo.CompanyId && p.EmployeeId == employeeInfo.EmployeeId)).ToArray();
                                var salaryData = new List<SalaryDataModel>();

                                for (int j = 0; j < s.Length; j++) {
                                    //TODO: Add % calculation from basic pay
                                    if (Regex.IsMatch(s[j].AdjustmentName, @"Shift [0-9]+")) {
                                        //TODO: Add code for checking shift exists
                                        
                                        var shiftNo = int.Parse(s[j].AdjustmentName.Split(' ')[1]);

                                        salaryData.Add(new SalaryDataModel {
                                            name = s[j].AdjustmentName,
                                            type = (s[j].AdjustmentValue >= 0) ? "+" : "-",
                                            value = Math.Abs(s[j].AdjustmentValue) * empAttendance.shift[shiftNo - 1]
                                        });
                                    } else {
                                        salaryData.Add(new SalaryDataModel {
                                            name = s[j].AdjustmentName,
                                            type = (s[j].AdjustmentValue >= 0) ? "+" : "-",
                                            value = Math.Abs(s[j].AdjustmentValue)
                                        });
                                    }
                                }

                                employeeData.Add(new EmployeeViewModel {
                                    id = employee.EmployeeId,
                                    name = employee.EmployeeName,
                                    doj = employee.DOJ,
                                    salary = salaryData.ToArray()
                                });

                                return Ok(new Message {
                                    data = employeeData,
                                    message = "Success"
                                });
                            } else {
                                return Ok(new Message {
                                    data = null,
                                    message = "Invalid data entered"
                                });
                            }
                        }

                        return Ok(new Message {
                            data = null,
                            message = "Invalid data entered"
                        });
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
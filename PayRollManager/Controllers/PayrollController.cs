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

        // GET: api/Payroll
        [HttpGet]
        public IHttpActionResult Calculate(String token, String attendanceJson) {
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
                                var basicPay = s.FirstOrDefault((p) => (p.AdjustmentName == "Basic"));

                                if (basicPay != null) {
                                    var history = db.Payroll_History.Where((p) => (p.CompanyId == employeeInfo.CompanyId && p.EmployeeId == employeeInfo.EmployeeId && p.Month.Month == DateTime.Now.Month)).ToArray();

                                    for (int j = 0; j < s.Length; j++) {
                                        if (Regex.IsMatch(s[j].AdjustmentName, @"Shift [0-9]+")) {
                                            var shiftNo = int.Parse(s[j].AdjustmentName.Split(' ')[1]);

                                            if (shiftNo <= empAttendance.shift.Length) {
                                                salaryData.Add(new SalaryDataModel {
                                                    name = s[j].AdjustmentName,
                                                    type = (s[j].AdjustmentValue >= 0) ? "+" : "-",
                                                    value = (s[j].AdjustmentType == "#") ? Math.Abs(s[j].AdjustmentValue) * empAttendance.shift[shiftNo - 1] : Math.Abs(s[j].AdjustmentValue * basicPay.AdjustmentValue / 100) * empAttendance.shift[shiftNo - 1]
                                                });
                                            } else {
                                                return Ok(new Message {
                                                    data = null,
                                                    message = "Shift length invalid"
                                                });
                                            }
                                        } else {
                                            salaryData.Add(new SalaryDataModel {
                                                name = s[j].AdjustmentName,
                                                type = (s[j].AdjustmentValue >= 0) ? "+" : "-",
                                                value = (s[j].AdjustmentType == "#") ? Math.Abs(s[j].AdjustmentValue) : Math.Abs(s[j].AdjustmentValue * basicPay.AdjustmentValue / 100)
                                            });
                                        }
                                    }

                                    for (int j = 0; j < history.Length; j++) {
                                        db.Payroll_History.Remove(history[j]);
                                    }

                                    for (int j = 0; j < salaryData.Count; j++) {
                                        db.Payroll_History.Add(new Payroll_History {
                                            CompanyId = employeeInfo.CompanyId,
                                            EmployeeId = employeeInfo.EmployeeId,
                                            Month = DateTime.Now,
                                            AdjustmentName = salaryData[j].name,
                                            AdjustmentType = salaryData[j].type,
                                            AdjustmentValue = salaryData[j].value
                                        });
                                    }

                                    employeeData.Add(new EmployeeViewModel {
                                        id = employeeInfo.EmployeeId,
                                        name = employeeInfo.EmployeeName,
                                        doj = employeeInfo.DOJ,
                                        salary = salaryData.ToArray()
                                    });
                                } else {
                                    return Ok(new Message {
                                        data = null,
                                        message = "Employee does not have a Basic Pay"
                                    });
                                }
                            } else {
                                return Ok(new Message {
                                    data = null,
                                    message = "Invalid data entered"
                                });
                            }
                        }

                        if(employeeData.Count == attendanceInput.attendance.Length) {
                            db.SaveChangesAsync();
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
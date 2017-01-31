using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayRollManager.Models;
using System.Configuration;
using System.Data.Entity;
using System.Text.RegularExpressions;

namespace PayRollManager.Controllers {
    public class PayrollHistoryController : ApiController {
        PayRollManagerEntities db = new PayRollManagerEntities();

        //GET: /api/PayrollHistory
        [HttpGet]
        public IHttpActionResult EmployeePayroll(String token, String date, String monthly) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));

                if (employee != null) {
                    var d = DateTime.Parse(date);
                    var history = db.Payroll_History.Where((p) => (p.CompanyId == employee.CompanyId && p.EmployeeId == employee.EmployeeId && p.Date.Month == d.Month && p.Date.Year == d.Year)).ToArray();

                    if(history.Length != 0) {
                        var salaryData = new List<SalaryDataModel>();

                        if (monthly == "y") {
                            for (int i = 0; i < history.Length; i++) {
                                salaryData.Add(new SalaryDataModel {
                                    name = history[i].AdjustmentName,
                                    type = history[i].AdjustmentType,
                                    value = history[i].AdjustmentValue
                                });
                            }
                        } else if (monthly == "n") {
                            var attendance = db.Attendance_Details.Where((p) => (p.CompanyId == employee.CompanyId && p.EmployeeId == employee.EmployeeId && p.Date.Month == d.Month && p.Date.Year == d.Year));

                            for (int i = 0; i < history.Length; i++) {
                                if (Regex.IsMatch(history[i].AdjustmentName, @"Shift [0-9]+")) {
                                    var shiftNo = int.Parse(history[i].AdjustmentName.Substring(6));
                                    var adjustmentValue = history[i].AdjustmentValue / attendance.Count((p) => (p.Shift == shiftNo));

                                    salaryData.Add(new SalaryDataModel {
                                        name = history[i].AdjustmentName,
                                        type = history[i].AdjustmentType,
                                        value = adjustmentValue 
                                    });
                                } else {
                                    var adjustmentValue = history[i].AdjustmentValue / attendance.Count((p) => (p.Shift > 0));

                                    salaryData.Add(new SalaryDataModel {
                                        name = history[i].AdjustmentName,
                                        type = history[i].AdjustmentType,
                                        value = adjustmentValue
                                    });
                                }
                            }
                        } else {
                            return Ok(new Message {
                                data = null,
                                message = "Invalid option entered"
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
        public IHttpActionResult CompanyPayroll(String token, int companyId, String date, String monthly) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    var d = DateTime.Parse(date);
                    var employeeList = db.Payroll_History.Where((p) => (p.CompanyId == companyId && p.Date.Month == d.Month && p.Date.Year == d.Year)).Select((p) => (p.EmployeeId)).Distinct().ToArray();
                    if (employeeList.Length != 0) {
                        var employeeData = new List<EmployeeViewModel>();
                        
                        for (int i = 0; i < employeeList.Length; i++) {
                            var salaryData = new List<SalaryDataModel>();
                            var employeeId = employeeList[i];
                            var employeeInfo = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId));
                            var history = db.Payroll_History.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId && p.Date.Month == d.Month && p.Date.Year == d.Year)).ToArray();

                            if (monthly == "y") {
                                for (int j = 0; j < history.Length; j++) {
                                    salaryData.Add(new SalaryDataModel {
                                        name = history[j].AdjustmentName,
                                        type = history[j].AdjustmentType,
                                        value = history[j].AdjustmentValue
                                    });
                                }
                            } else if (monthly == "n") {
                                var attendance = db.Attendance_Details.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId && p.Date.Month == d.Month && p.Date.Year == d.Year));

                                for (int j = 0; j < history.Length; j++) {
                                    if (Regex.IsMatch(history[j].AdjustmentName, @"Shift [0-9]+")) {
                                        var shiftNo = int.Parse(history[j].AdjustmentName.Substring(6));
                                        var adjustmentValue = history[j].AdjustmentValue / attendance.Count((p) => (p.Shift == shiftNo));

                                        salaryData.Add(new SalaryDataModel {
                                            name = history[j].AdjustmentName,
                                            type = history[j].AdjustmentType,
                                            value = adjustmentValue
                                        });
                                    } else {
                                        var adjustmentValue = history[j].AdjustmentValue / attendance.Count((p) => (p.Shift > 0));

                                        salaryData.Add(new SalaryDataModel {
                                            name = history[j].AdjustmentName,
                                            type = history[j].AdjustmentType,
                                            value = adjustmentValue
                                        });
                                    }
                                }
                            } else {
                                return Ok(new Message {
                                    data = null,
                                    message = "Invalid option entered"
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

        //GET: /api/PayrollHistory
        [HttpGet]
        public IHttpActionResult CompanyEmployeePayroll(String token, int companyId, String employeeId, String date, String monthly) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    var d = DateTime.Parse(date);
                    var employeeInfo = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId));

                    if (employeeInfo != null) {
                        var salaryData = new List<SalaryDataModel>();
                        var history = db.Payroll_History.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId && p.Date.Month == d.Month && p.Date.Year == d.Year)).ToArray();

                        if (history.Length != 0) {
                            if (monthly == "y") {
                                for (int j = 0; j < history.Length; j++) {
                                    salaryData.Add(new SalaryDataModel {
                                        name = history[j].AdjustmentName,
                                        type = history[j].AdjustmentType,
                                        value = history[j].AdjustmentValue
                                    });
                                }
                            } else if (monthly == "n") {
                                var attendance = db.Attendance_Details.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId && p.Date.Month == d.Month && p.Date.Year == d.Year));

                                for (int j = 0; j < history.Length; j++) {
                                    if (Regex.IsMatch(history[j].AdjustmentName, @"Shift [0-9]+")) {
                                        var shiftNo = int.Parse(history[j].AdjustmentName.Substring(6));
                                        var adjustmentValue = history[j].AdjustmentValue / attendance.Count((p) => (p.Shift == shiftNo));

                                        salaryData.Add(new SalaryDataModel {
                                            name = history[j].AdjustmentName,
                                            type = history[j].AdjustmentType,
                                            value = adjustmentValue
                                        });
                                    } else {
                                        var adjustmentValue = history[j].AdjustmentValue / attendance.Count((p) => (p.Shift > 0));

                                        salaryData.Add(new SalaryDataModel {
                                            name = history[j].AdjustmentName,
                                            type = history[j].AdjustmentType,
                                            value = adjustmentValue
                                        });
                                    }
                                }
                            } else {
                                return Ok(new Message {
                                    data = null,
                                    message = "Invalid option entered"
                                });
                            }

                            return Ok(new Message {
                                data = new EmployeeViewModel {
                                    id = employeeInfo.EmployeeId,
                                    name = employeeInfo.EmployeeName,
                                    doj = employeeInfo.DOJ,
                                    salary = salaryData.ToArray()
                                },
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
                            message = "The specified employee does not exist"
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

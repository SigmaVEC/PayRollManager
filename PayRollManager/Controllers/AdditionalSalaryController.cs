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
    public class AdditionalSalaryController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/AdditionalSalary
        [HttpGet]
        public IHttpActionResult AdditionalSalaryEdit(String token, String action, String dataJson) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    try {
                        var salaryDataInput = new JavaScriptSerializer().Deserialize<AdditionalSalaryInputModel>(dataJson);

                        if (action == "add") {
                            for (int i = 0; i < salaryDataInput.additionalSalary.Length; i++) {
                                var empAdditionalSalary = salaryDataInput.additionalSalary[i];

                                for (int j = 0; j < empAdditionalSalary.bonus.Length; j++) {
                                    var bonus = empAdditionalSalary.bonus[j];
                                    var employeeInfo = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == empAdditionalSalary.companyId && p.EmployeeId == empAdditionalSalary.employeeId));

                                    if (employeeInfo != null) {
                                        if (DateTime.Compare(bonus.applyDate, DateTime.Now) > 0) {
                                            if (bonus.availableRepeats >= -1) {
                                                if (bonus.type == "#" || (bonus.type == "%" && bonus.value <= 100)) {
                                                    db.Salary_Bonus.Add(new Salary_Bonus {
                                                        CompanyId = employeeInfo.CompanyId,
                                                        EmployeeId = employeeInfo.EmployeeId,
                                                        ApplyDate = bonus.applyDate,
                                                        BonusName = bonus.name,
                                                        BonusType = bonus.type,
                                                        BonusValue = bonus.value,
                                                        TargetAttendance = bonus.targetAttendance,
                                                        AvailableRepeats = bonus.availableRepeats
                                                    });
                                                } else {
                                                    return Ok(new Message {
                                                        data = null,
                                                        message = "bonus value or type invalid"
                                                    });
                                                }
                                            } else {
                                                return Ok(new Message {
                                                    data = null,
                                                    message = "Invalid number of repeats"
                                                });
                                            }
                                        } else {
                                            return Ok(new Message {
                                                data = null,
                                                message = "Apply date cannot be in the past"
                                            });
                                        }
                                    } else {
                                        return Ok(new Message {
                                            data = null,
                                            message = "Employee not found"
                                        });
                                    }
                                }

                                for (int j = 0; j < empAdditionalSalary.increments.Length; j++) {
                                    var increment = empAdditionalSalary.increments[j];
                                    var employeeInfo = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == empAdditionalSalary.companyId && p.EmployeeId == empAdditionalSalary.employeeId));

                                    if (employeeInfo != null) {
                                        if (DateTime.Compare(increment.applyDate, DateTime.Now) > 0) {
                                            if (increment.type == "#" || (increment.type == "%" && increment.value <= 100)) {
                                                db.Salary_Increments.Add(new Salary_Increments {
                                                    CompanyId = employeeInfo.CompanyId,
                                                    EmployeeId = employeeInfo.EmployeeId,
                                                    ApplyDate = increment.applyDate,
                                                    IncrementName = increment.name,
                                                    IncrementType = increment.type,
                                                    IncrementValue = increment.value
                                                });
                                            } else {
                                                return Ok(new Message {
                                                    data = null,
                                                    message = "bonus value or type invalid"
                                                });
                                            }
                                        } else {
                                            return Ok(new Message {
                                                data = null,
                                                message = "Apply date cannot be in the past"
                                            });
                                        }
                                    } else {
                                        return Ok(new Message {
                                            data = null,
                                            message = "Employee not found"
                                        });
                                    }
                                }
                            }

                            db.SaveChangesAsync();
                            return Ok(new Message {
                                data = null,
                                message = "Success"
                            });
                        } else if (action == "remove") {
                            for (int i = 0; i < salaryDataInput.additionalSalary.Length; i++) {
                                var empAdditionalSalary = salaryDataInput.additionalSalary[i];

                                for (int j = 0; j < empAdditionalSalary.bonus.Length; j++) {
                                    var empBonus = db.Salary_Bonus.FirstOrDefault((p) => (p.CompanyId == empAdditionalSalary.companyId && p.EmployeeId == empAdditionalSalary.employeeId && p.ApplyDate == empAdditionalSalary.bonus[j].applyDate && p.BonusName == empAdditionalSalary.bonus[j].name));
                                    db.Salary_Bonus.Remove(empBonus);
                                }

                                for (int j = 0; j < empAdditionalSalary.increments.Length; j++) {
                                    var empIncrement = db.Salary_Increments.FirstOrDefault((p) => (p.CompanyId == empAdditionalSalary.companyId && p.EmployeeId == empAdditionalSalary.employeeId && p.ApplyDate == empAdditionalSalary.increments[j].applyDate && p.IncrementName == empAdditionalSalary.increments[j].name));
                                    db.Salary_Increments.Remove(empIncrement);
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
                                message = "Invalid action"
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

        // GET: api/AdditionalSalary
        [HttpGet]
        public IHttpActionResult AdditionalSalaryView(String token, String action, int companyId, String employeeId) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    if (action == "view") {
                        var employeeInfo = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId));

                        if (employeeInfo != null) {
                            var bonus = db.Salary_Bonus.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId)).ToArray();
                            var increments = db.Salary_Increments.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId)).ToArray();
                            var salaryBonus = new List<SalaryBonusModel>();
                            var salaryIncrements = new List<SalaryIncrementsModel>();

                            for (int i = 0; i < bonus.Length; i++) {
                                salaryBonus.Add(new SalaryBonusModel {
                                    applyDate = bonus[i].ApplyDate,
                                    name = bonus[i].BonusName,
                                    type = bonus[i].BonusType,
                                    value = bonus[i].BonusValue,
                                    targetAttendance = bonus[i].TargetAttendance,
                                    availableRepeats = bonus[i].AvailableRepeats
                                });
                            }

                            for (int i = 0; i < increments.Length; i++) {
                                salaryIncrements.Add(new SalaryIncrementsModel {
                                    applyDate = increments[i].ApplyDate,
                                    name = increments[i].IncrementName,
                                    type = increments[i].IncrementType,
                                    value = increments[i].IncrementValue
                                });
                            }

                            return Ok(new Message {
                                data = new AdditionalSalaryDataModel {
                                    companyId = companyId,
                                    employeeId = employeeId,
                                    bonus = salaryBonus.ToArray(),
                                    increments = salaryIncrements.ToArray()
                                },
                                message = "Success"
                            });
                        } else {
                            return Ok(new Message {
                                data = null,
                                message = "Employee not found"
                            });
                        }
                    } else {
                        return Ok(new Message {
                            data = null,
                            message = "Invalid action"
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

﻿using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayRollManager.Controllers {
    public class RemoveCompanyController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/RemoveEmployee
        [HttpGet]
        public IHttpActionResult RemoveCompany(String token, int companyId) {
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    var company = db.Company_Info.FirstOrDefault((p) => (p.CompanyId == companyId));
                    db.Company_Info.Remove(company);
                    db.SaveChangesAsync();

                    var companyEmployees = db.Employee_Info.Where((p) => (p.CompanyId == companyId));
                    foreach(var i in companyEmployees) {
                        db.Employee_Info.Remove(i);
                    }
                    db.SaveChangesAsync();

                    var companySalaries = db.Employee_Salary.Where((p) => (p.CompanyId == companyId));
                    foreach (var i in companySalaries) {
                        db.Employee_Salary.Remove(i);
                    }
                    db.SaveChangesAsync();

                    var companySessions = db.Session_Tokens.Where((p) => (p.CompanyId == companyId));
                    foreach (var i in companySessions) {
                        db.Session_Tokens.Remove(i);
                    }
                    db.SaveChangesAsync();

                    return Ok(new Message {
                        data = null,
                        message = "Success"
                    });
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
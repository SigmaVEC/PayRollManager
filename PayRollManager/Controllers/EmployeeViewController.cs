﻿using PayRollManager.Models;
using System;
using System.Collections.Generic;
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
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token));

            if(session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId));
                var s = db.Employee_Salary.Where((p) => (p.CompanyId == employee.CompanyId && p.EmployeeId == employee.EmployeeId)).ToArray();
                var salaryData = new List<SalaryDataModel>();

                for(int i = 0; i < s.Length; i++) {
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
    }
}
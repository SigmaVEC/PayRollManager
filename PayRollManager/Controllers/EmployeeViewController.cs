using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

namespace PayRollManager.Controllers {
    public class EmployeeViewController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/EmployeeView
        [ResponseType(typeof(Employee_Info))]
        [HttpGet]
        public IHttpActionResult View(int companyId, String employeeId, String password) {
            var company = db.Company_Info.FirstOrDefaultAsync((p) => (p.CompanyId == companyId));

            if (company.Result != null) {
                var employee = db.Employee_Info.FirstOrDefaultAsync((p) => (p.EmployeeId == employeeId && p.Password == password));

                if (employee.Result != null) {
                    EmployeeViewModel ev = new EmployeeViewModel {
                        id = employee.Result.EmployeeId,
                        name = employee.Result.EmployeeName,
                        doj = employee.Result.DOJ,
                        salary = db.Employee_Salary.Where((p) => (p.CompanyId == companyId && p.EmployeeId == employeeId)).ToArray()
                    };

                    return Ok(ev);
                } else {
                    return Ok(employee.Result);
                }
            } else {
                return Ok(company.Result);
            }
        }
    }
}

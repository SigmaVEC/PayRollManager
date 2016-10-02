using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class EmployeeDataModel {
        public int companyId { get; set; }
        public string employeeId { get; set; }
        public string name { get; set; }
        public System.DateTime date { get; set; }
        public string isAdmin { get; set; }
        public string password { get; set; }
        public SalaryDataModel[] salary { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class EmployeeDataModel {
        public int companyId { get; set; }
        public String employeeId { get; set; }
        public String name { get; set; }
        public System.DateTime date { get; set; }
        public String isAdmin { get; set; }
        public String password { get; set; }
        public PersonalDataModel[] personal { get; set; }
        public SalaryDataModel[] salary { get; set; }
    }
}
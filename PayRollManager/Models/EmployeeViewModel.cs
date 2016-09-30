using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class EmployeeViewModel {
        public String id { get; set; }
        public String name { get; set; }
        public DateTime doj { get; set; }
        public Employee_Salary[] salary { get; set; }
    }
}
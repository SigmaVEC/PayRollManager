using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class EmployeeViewModel {
        public String id { get; set; }
        public String name { get; set; }
        public DateTime doj { get; set; }
        public Nullable<DateTime> dol { get; set; }
        public PersonalDataModel[] personal { get; set; }
        public SalaryDataModel[] salary { get; set; }
    }
}
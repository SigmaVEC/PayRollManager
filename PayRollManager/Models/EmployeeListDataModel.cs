using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class EmployeeListDataModel {
        public String Message { get; set; }
        public EmployeeDataModel[] employee { get; set; }
    }
}
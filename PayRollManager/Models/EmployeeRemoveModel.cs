using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class EmployeeRemoveModel {
        public int companyId { get; set; }
        public String employeeId { get; set; }
        public DateTime dol { get; set; }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class EmployeeRemoveModel {
        public int companyId { get; set; }
        public string employeeId { get; set; }
        public DateTime dol { get; set; }
    }
}
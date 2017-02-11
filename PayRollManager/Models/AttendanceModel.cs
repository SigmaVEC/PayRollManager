using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class AttendanceModel {
        public int companyId { get; set; }
        public String employeeId { get; set; }
        public DateTime date { get; set; }
        public String[] shifts { get; set; }
    }
}
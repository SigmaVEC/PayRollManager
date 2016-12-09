using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class SalaryIncrementsModel {
        public DateTime applyDate { get; set; }
        public String name { get; set; }
        public String type { get; set; }
        public double value { get; set; }
    }
}
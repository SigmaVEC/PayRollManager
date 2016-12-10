using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class AdditionalSalaryDataModel {
        public int companyId { get; set; }
        public String employeeId { get; set; }
        public SalaryBonusModel[] bonus { get; set; }
        public SalaryIncrementsModel[] increments { get; set; }
    }
}
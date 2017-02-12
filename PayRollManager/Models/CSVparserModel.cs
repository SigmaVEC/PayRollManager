using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace PayRollManager.Models {
    public class CSVparserModel {
        public int companyId { get; set; }
        public DateTime date { get; set; }
        public String token { get; set; }
    }
}
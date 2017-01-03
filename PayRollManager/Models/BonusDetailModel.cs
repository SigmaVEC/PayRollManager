using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayRollManager.Models
{
    public class BonusDetailModel
    {
        public int companyId { get; set; }
        public String employeeId { get; set; }
        public System.DateTime date { get; set; }
        public String type { get; set; }
        public String isrepeating { get; set; }
        public String bonustype { get; set; }
        public float bonusvalue { get; set; }

    }
}
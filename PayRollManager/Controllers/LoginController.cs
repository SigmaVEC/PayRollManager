using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using PayRollManager.Models;

namespace PayRollManager.Controllers {
    public class LoginController : ApiController {
        Details[] d = new Details[] {   
            //database values goes here.
            //dummy data
            new Details {id="l5382A",password="leochan" },
            new Details {id="78232H", password="rhodio" }

        };

        public IHttpActionResult GetById(String id, String password) {
            var res = d.FirstOrDefault((p) => (p.id == id && p.password == password));

            if (res != null) {
                return Ok(res);
            } else {
                return NotFound();
            }
        }
    }
}

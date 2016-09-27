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

        public String Authenticate(String id, String password) {
            var res = d.FirstOrDefault((p) => p.id == id);
            if (res == null) {
                return "User ID is invalid";
            } else {
                String pass = res.password;
                if (pass.Equals(password)) {
                    return "Authentication Successful";
                } else {
                    return "Password is invalid";
                }
            }
        }
    }
}

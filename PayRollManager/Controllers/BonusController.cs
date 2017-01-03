using PayRollManager.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using System.Web.Script.Serialization;

namespace PayRollManager.Controllers
{
    public class BonusController : ApiController
    {
        private PayRollManagerEntities db = new PayRollManagerEntities();
        //GET: api/Bonus
        [HttpGet]
        public IHttpActionResult BonusCalc(String token, String bonusJSON)
        {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));
            if (session != null)
            {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));
                if (employee != null)
                {
                    try
                    {
                        var bonusDetails = new JavaScriptSerializer().Deserialize<BonusDetailModel>(bonusJSON);
                        var empsalary = db.Employee_Salary.Where(p => (p.CompanyId == Convert.ToInt32(bonusDetails.companyId))).ToList();
                        if (empsalary != null)
                        {
                            for (int i = 0; i < empsalary.Capacity; i++)
                            {
                                var EmployeeID = empsalary[i].EmployeeId;
                                var b = new Bonus_Details
                                {
                                    CompanyId = bonusDetails.companyId,
                                    EmployeeId = bonusDetails.employeeId,
                                    Date = bonusDetails.date,
                                    Type = bonusDetails.type,
                                    IsRepeating = bonusDetails.isrepeating,
                                    BonusType = bonusDetails.bonustype,
                                    BonusValue = bonusDetails.bonusvalue
                                };
                                db.Bonus_Details.Add(b);

                            }
                            db.SaveChangesAsync();
                            return Ok(new Message
                            {
                                data = null,
                                message = "Success"
                            });
                        }
                        else
                        {
                            return Ok(new Message
                            {
                                data = null,
                                message = "No such Company exists."
                            });

                        }
                    }
                    catch (System.ArgumentException e)
                    {
                        return Ok(new Message
                        {
                            data = null,
                            message = "JSON format is invalid"
                        });
                    }
                }
                else
                {
                    return Ok(new Message
                    {
                        data = null,
                        message = "You do not have permission to perform this operation."
                    });
                }

            }
            else
            {
                return Ok(new Message
                {
                    data = null,
                    message = "Session Token is invalid"
                });
            }
        
        }
    }
}

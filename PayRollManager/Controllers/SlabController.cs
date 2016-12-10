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

namespace PayRollManager.Controllers {
    public class SlabController : ApiController {
        private PayRollManagerEntities db = new PayRollManagerEntities();

        // GET: api/Slab
        [HttpGet]
        public IHttpActionResult ViewSlab(String token, String action, int companyId) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    if (action == "view") {
                        var slabs = db.Salary_Slab.Where((p) => (p.CompanyId == companyId)).ToArray();

                        return Ok(new Message {
                            data = slabs,
                            message = "Success"
                        });
                    } else {
                        return Ok(new Message {
                            data = null,
                            message = "Invalid action"
                        });
                    }
                } else {
                    return Ok(new Message {
                        data = null,
                        message = "You do not have permission to perform this operation"
                    });
                }
            } else {
                return Ok(new Message {
                    data = null,
                    message = "Session Token is invalid"
                });
            }
        }

        // GET: api/Slab
        [HttpGet]
        public IHttpActionResult EditSlab(String token, String action, String slabJson) {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));

            if (session != null) {
                var employee = db.Employee_Info.FirstOrDefault((p) => (p.CompanyId == session.CompanyId && p.EmployeeId == session.EmployeeId && p.IsAdmin == "y"));

                if (employee != null) {
                    try {
                        var slabInput = new JavaScriptSerializer().Deserialize<SlabInputModel>(slabJson);

                        if (action == "add") {
                            for (int i = 0; i < slabInput.slab.Length; i++) {
                                var slab = slabInput.slab[i];
                                var companyInfo = db.Company_Info.FirstOrDefault((p) => (p.CompanyId == slab.CompanyId));

                                if (companyInfo != null) {
                                    if (slab.FromAmount < slab.ToAmount && slab.Value >= 0) {
                                        db.Salary_Slab.Add(slab);
                                    } else {
                                        return Ok(new Message {
                                            data = null,
                                            message = "Invalid slab details"
                                        });
                                    }
                                } else {
                                    return Ok(new Message {
                                        data = null,
                                        message = "Invalid CompanyId"
                                    });
                                }
                            }

                            return Ok(new Message {
                                data = null,
                                message = "Success"
                            });
                        } else if (action == "remove") {
                            for (int i = 0; i < slabInput.slab.Length; i++) {
                                var slab = slabInput.slab[i];
                                var companyInfo = db.Company_Info.FirstOrDefault((p) => (p.CompanyId == slab.CompanyId));

                                if (companyInfo != null) {
                                    db.Salary_Slab.Remove(slab);
                                } else {
                                    return Ok(new Message {
                                        data = null,
                                        message = "Invalid CompanyId"
                                    });
                                }
                            }

                            return Ok(new Message {
                                data = null,
                                message = "Success"
                            });
                        } else {
                            return Ok(new Message {
                                data = null,
                                message = "Invalid action"
                            });
                        }
                    } catch (System.ArgumentException) {
                        return Ok(new Message {
                            data = null,
                            message = "JSON format is invalid"
                        });
                    }
                } else {
                    return Ok(new Message {
                        data = null,
                        message = "You do not have permission to perform this operation"
                    });
                }
            } else {
                return Ok(new Message {
                    data = null,
                    message = "Session Token is invalid"
                });
            }
        }
    }
}
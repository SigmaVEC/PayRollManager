﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace PayRollManager.Controllers
{
    public class CSVParsingController : ApiController
    {
        private PayRollManagerEntities db = new PayRollManagerEntities();
        string path;
        // POST: api/Index
        [HttpPost]
        public ActionResult Index(HttpPostedFileBase file)
        {
            if (file != null && file.ContentLength > 0)
                try
                {
                    path = Path.Combine(Server.MapPath("~/Files"),
                                               Path.GetFileName(file.FileName));
                    file.SaveAs(path);
                   
                    ViewBag.Message = "File uploaded successfully";
                }
                catch (Exception ex)
                {
                    ViewBag.Message = "ERROR:" + ex.Message.ToString();
                }
            else
            {
                ViewBag.Message = "You have not specified a file.";
            }
            return View();
        }
        // GET: api/CSVParser
        [HttpGet]
        public IHttpActionResult CSVParser(String token)
        {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));
            if(session!=null)
            {
                // Get the file's text.
                string whole_file = System.IO.File.ReadAllText(path);

                // Split into lines.
                whole_file = whole_file.Replace('\n', '\r');
                string[] lines = whole_file.Split(new char[] { '\r' },
                    StringSplitOptions.RemoveEmptyEntries);

                // See how many rows and columns there are.
                int num_rows = lines.Length;
                int num_cols = lines[0].Split(',').Length;

                // Allocate the data array.
                string[,] values = new string[num_rows, num_cols];

                // Load the array.
                for (int r = 0; r < num_rows; r++)
                {
                    string[] line_r = lines[r].Split(',');
                    for (int c = 0; c < num_cols; c++)
                    {
                        values[r, c] = line_r[c];
                    }
                }

                var dictionaryTransformed = new List<Dictionary<string,string>>();
                var jsondict = new List<string>();
                for (int r = 1; r < 3; r++)
                    for (int r=1;r<num_rows;r++)
                {
                    dictionaryTransformed.Add(new Dictionary<string, string>());



                    dictionaryTransformed[r - 1].Add(values[0, 0], values[r, 0]);
                    dictionaryTransformed[r - 1].Add(values[0, 1], values[r, 1]);
                    dictionaryTransformed[r - 1].Add(values[0, 2], values[r, 2]);
                    jsondict.Add(Newtonsoft.Json.JsonConvert.SerializeObject(dictionaryTransformed[r - 1]));



                }
                var result = new Dictionary<string, List<string>>();
                result["attendance"] = jsondict;
                var jsonresult=Newtonsoft.Json.JsonConvert.SerializeObject(result);
                return Ok(new Message
                {
                    data = json,
                    message = "Success"
                });

            }
        }
    }
}
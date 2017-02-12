
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web;
using System.Web.Http;
using PayRollManager.Models;

namespace PayRollManager.Controllers
{
    public class CSVParserController : ApiController
    {
        private PayRollManagerEntities db = new PayRollManagerEntities();
        string path = "";

        [HttpPost]
        public async Task<HttpResponseMessage> PostFile(CSVparserModel data)
        {
            var companyId = data.companyId;
            var date = data.date;
            string token = data.token;
            var jsonresult = " ";
            // Check if the request contains multipart/form-data.
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));
            if (session != null)
            {
                if (!Request.Content.IsMimeMultipartContent())
                {
                    throw new HttpResponseException(HttpStatusCode.UnsupportedMediaType);
                }

                string root = HttpContext.Current.Server.MapPath("~/App_Data");
                Trace.WriteLine(root);
                var provider = new MultipartFormDataStreamProvider(root);

                try
                {
                    // Read the form data.
                    await Request.Content.ReadAsMultipartAsync(provider);

                    // This illustrates how to get the file names.

                    foreach (MultipartFileData file in provider.FileData)
                    {
                        Trace.WriteLine(file.Headers.ContentDisposition.FileName);
                        //To View your file location in server.
                        Trace.WriteLine("Server file path: " + file.LocalFileName);
                        path = file.LocalFileName;



                        // Get the file's text.
                        try
                        {

                            string whole_file = File.ReadAllText(path);

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
                            for (int r = 1; r < num_rows; r++)
                            {
                                string[] line_r = lines[r].Split(',');
                                for (int c = 0; c < num_cols; c++)
                                {
                                    values[r, c] = line_r[c];
                                }
                            }

                            var dictionaryTransformed = new List<Dictionary<string, object>>();
                            var jsondict = new List<string>();

                            for (int r = 1; r < num_rows; r++)
                            {

                                dictionaryTransformed.Add(new Dictionary<string, object>());
                                dictionaryTransformed[r - 1].Add("companyId", companyId);

                                var shift = new List<string>();
                                for (int c = 0; c < num_cols; c++)
                                {
                                    if (c == 0)
                                    {
                                        dictionaryTransformed[r - 1].Add("employeeId", values[r, c]);
                                    }
                                    else
                                    {
                                        shift.Add(values[r, c]);

                                    }


                                }
                                dictionaryTransformed[r - 1].Add("date", DateTime.Now);
                                dictionaryTransformed[r - 1].Add("shift", shift);
                                jsondict.Add(Newtonsoft.Json.JsonConvert.SerializeObject(dictionaryTransformed[r - 1]));

                            }
                            var result = new Dictionary<string, List<string>>();
                            result["attendance"] = jsondict;
                            jsonresult = Newtonsoft.Json.JsonConvert.SerializeObject(result);

                            
                        }
                        catch
                        {
                            return Request.CreateResponse(HttpStatusCode.OK, "Cannot open File");

                        }


                    }
                    return Request.CreateResponse(HttpStatusCode.OK, jsonresult);

                }
            

            catch (System.Exception e)
            {
                return Request.CreateErrorResponse(HttpStatusCode.InternalServerError, e);
            }
        }
        else
        {
               return Request.CreateResponse(HttpStatusCode.OK, "Session Token is invalid");
         }
      }

        }

    }



using System;
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
        // GET: api/CSVParser
        [HttpGet]
        public IHttpActionResult CSVParser(String token, String filename)
        {
            var tokenLifetime = int.Parse(ConfigurationManager.AppSettings["tokenLifetime"]);
            var session = db.Session_Tokens.FirstOrDefault((p) => (p.SessionToken == token && DbFunctions.DiffHours(DateTime.Now, p.Timestamp) < tokenLifetime));
            if(session!=null)
            {
                // Get the file's text.
                string whole_file = System.IO.File.ReadAllText(filename);

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
                for(int r=1;r<num_rows;r++)
                {
                    var addvalues = new Dictionary<string, string>();

                    addvalues.Add(values[0, 0], values[r, 0]);
                    addvalues.Add(values[0, 1], values[r, 1]);
                    addvalues.Add(values[0, 2], values[r, 2]);
                    dictionaryTransformed.Add(addvalues.Select(item => new { item.Key, Value = addvalues[item.Key] }));


                }
                var result = new Dictionary<string, List<Dictionary<string, string>>>();
                result["attendance"] = dictionaryTransformed;
                var convertedresult = result.ToDictionary(item => item.Key.ToString(), item => item.Value.ToString());

                var json = new JavaScriptSerializer().Serialize(convertedresult);
                return Ok(new Message
                {
                    data = json,
                    message = "json converted output"
                });

            }
        }
    }
}
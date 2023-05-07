using EFCore.BulkExtensions;
using ETL.Data;
using ETL.DBModels;
using ETL.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;
using System.Text;
using System.Text.RegularExpressions;

namespace ETL.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        public RainfallContext _context;

        public HomeController(ILogger<HomeController> logger, RainfallContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index()
        {
            String cookie = Request.Cookies["ETL_JBA"];

            if (cookie == null)
            {
                var cookieOptions = new CookieOptions();
                cookieOptions.Secure = false;
                cookieOptions.Expires = DateTime.Now.AddDays(30);
                Response.Cookies.Append("ETL_JBA", Guid.NewGuid().ToString(), cookieOptions);
            }

            if (_context.Rainfalls.Any(x => x.session == cookie))
            {
                return RedirectToAction("Results");
            }

            return View();
        }

        public IActionResult Results()
        {
            String cookie = Request.Cookies["ETL_JBA"];
            List<AnnualSummary> annualSummaries = _context.AnnualSummaries.Where(x => x.session == cookie).OrderBy(x => x.RecordYear).ToList();
            return View(annualSummaries);
        }

        [HttpPost]
        public IActionResult UploadData(IFormFile prefile)
        {
            String cookie = Request.Cookies["ETL_JBA"];
            _context.ChangeTracker.AutoDetectChangesEnabled = false;

            if (cookie == null)
            {
                return RedirectToAction("Error", new { error = "No session to upload data for. Ensure that cookies are enabled on the browser." });
            }

            if (prefile == null)
            {
                return RedirectToAction("Error", new { error = "Ensure that a file is selected" });
            }
            StreamReader reader = new StreamReader(prefile.OpenReadStream());
            List<Rainfall> rainfalls = new List<Rainfall>();
            string error = "Error reading file";
            decimal xref = -1;
            decimal yref = -1;
            int year_start = -1;
            int year = -1;
            int block = 0;
            try
            {
                while (reader.Peek() >= 0)
                {
                    error = "Error in Block " + block.ToString();
                    string prefile_line = reader.ReadLine();

                    if (prefile_line.Contains("[Years="))
                    {
                        year_start = int.Parse(prefile_line.Substring(prefile_line.IndexOf("[Years=") + 7, 4));
                    }

                    if (prefile_line.Contains("Grid-ref="))
                    {
                        string[] split = Regex.Split(prefile_line, @" +");
                        xref = decimal.Parse(split[1]);
                        yref = decimal.Parse(split[2]);
                        year = year_start;
                        block++;
                    }

                    //data lines are 5x12 = 60 long
                    if (xref != -1 && yref != -1 && year != -1 && prefile_line.Length == 60)
                    {
                        List<decimal> ds = new List<decimal>();
                        for (int i = 0; i < 60; i += 5)
                        {
                            decimal d;
                            if (decimal.TryParse(prefile_line.Substring(i, 5), out d))
                            {
                                ds.Add(d);
                            }
                        }

                        //if there are 12 numbers then we can make a year
                        if (ds.Count() == 12)
                        {
                            int month = 1;
                            foreach (decimal d in ds)
                            {
                                Rainfall rainfall = new Rainfall 
                                { 
                                    session = cookie,
                                    xref = xref,
                                    yref = yref,
                                    value = d,
                                    date = new DateTime(year, month, 1)
                                };
                                rainfalls.Add(rainfall);
                                month += 1;
                            }
                            year += 1;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return RedirectToAction("Error", new { error = error + " : " + ex.Message });
            }

            _context.BulkInsert(rainfalls);
            _context.SaveChanges();

            return RedirectToAction("Index");
        }


        [HttpPost]
        public IActionResult DeleteData()
        {
            String cookie = Request.Cookies["ETL_JBA"];
            List<Rainfall> rainfalls = _context.Rainfalls.Where(x => x.session == cookie).ToList();
            _context.BulkDelete(rainfalls);
            return RedirectToAction("Index");
        }

            public IActionResult Error(string error)
        {
            return View(model:error);
        }
    }
}
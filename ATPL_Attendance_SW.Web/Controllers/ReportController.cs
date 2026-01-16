using System.Data;
using System.Diagnostics;
using System.Drawing;
using ATPL_Attendance_SW.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;
using Microsoft.Reporting.NETCore;

namespace ATPL_Attendance_SW.Web.Controllers
{
    [Authorize(AuthenticationSchemes = "AttendanceCookie")]

    public class ReportController : Controller
    {
        private readonly ILogger<ReportController> _logger;
        private readonly DataUtility du;

        public ReportController(
        ILogger<ReportController> logger,
        IConfiguration config)
        {
            _logger = logger;
            du = new DataUtility(config);
        }
        public IActionResult Index()
        {
            return View();
        }



        public IActionResult HolidayReportPdf()
        {
            DataTable dt = du.GetDataTable("Sp_Get_HolidayList", null);

            LocalReport report = new LocalReport();

            string path = Path.Combine(
                Directory.GetCurrentDirectory(),
                "Reports",
                "Holidays.rdlc");

            report.ReportPath = path;

            report.DataSources.Clear();
            report.DataSources.Add(
                new ReportDataSource("DatasetHoliday", dt));

            byte[] pdf = report.Render("PDF");

            return File(pdf, "application/pdf", "HolidayReport.pdf");
        }

        public ActionResult HolidayReport()
        {
            DataTable dt = du.GetDataTable("Sp_Get_HolidayList", null);

            List<HolidayVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new HolidayVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Holiday = row["Holiday"].ToString(),
                    Date = Convert.ToDateTime(row["Date"]).ToString("yyyy-MM-dd"),
                    Description = row["Description"].ToString(),
                    Active = row["Active"].ToString() == "1"
                });
            }

            ViewBag.HolidayData = dt;
            return View();
        }
    }
}

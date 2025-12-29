using Microsoft.AspNetCore.Mvc;

namespace ATPL_Attendance_SW.Web.Controllers
{
    public class AttendanceController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

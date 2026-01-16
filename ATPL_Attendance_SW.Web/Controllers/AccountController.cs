using System.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ATPL_Attendance_SW.Web.Controllers
{
    public class AccountController : Controller
    {
        DataUtility du;

        public AccountController(IConfiguration config)
        {
            du = new DataUtility(config);
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public IActionResult Index()
        {
            return View();
        }

        [HttpGet]
        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Login()
        {
            //if (User.Identity.IsAuthenticated)
            //    return RedirectToAction("Index", "Home");

            return View();
        }

        [ResponseCache(Location = ResponseCacheLocation.None, NoStore = true)]

        public async Task<IActionResult> Login(string username, string password)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@Username", username),
            new SqlParameter("@Password", password)
        };

            DataTable dt = du.GetDataTable("Sp_Login_Authenticate", prms);

            if (dt.Rows.Count > 0)
            {

            string img = dt.Rows[0]["Emp_Img"] == DBNull.Value
            ? ""
            : dt.Rows[0]["Emp_Img"].ToString();

             var claims = new List<Claim>
            {
            new Claim(ClaimTypes.Name, dt.Rows[0]["UserName"].ToString()),
            new Claim(ClaimTypes.Role, dt.Rows[0]["Role"].ToString()),
            new Claim("UserImage", img)           };

                var identity = new ClaimsIdentity(claims, "AttendanceCookie");

                await HttpContext.SignInAsync(
                    "AttendanceCookie", 
                    new ClaimsPrincipal(identity),
                    new AuthenticationProperties
                    {
                        IsPersistent = true,
                        ExpiresUtc = DateTimeOffset.UtcNow.AddDays(7)
                    });

                return RedirectToAction("Index", "Home");
            }

            ViewBag.Error = "Invalid Username or Password";
            return View();
        }

        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync("AttendanceCookie");

            Response.Cookies.Delete(".AspNetCore.AttendanceCookie");

            return RedirectToAction("Login", "Account");
        }
    }
}

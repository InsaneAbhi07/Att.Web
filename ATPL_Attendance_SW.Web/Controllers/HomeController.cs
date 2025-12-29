using System.Data;
using System.Diagnostics;
using ATPL_Attendance_SW.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ATPL_Attendance_SW.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly DataUtility du;

        public HomeController(
            ILogger<HomeController> logger,
            IConfiguration config)
        {
            _logger = logger;
            du = new DataUtility(config);
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        public IActionResult AddEmployee()
        {
            return View();
        }

        public IActionResult AddHolidays()
        {
            return View();
        }

        public IActionResult AddDepartment(DepartmentVM model)
        {
            SqlParameter[] prms =
        {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@Department", model.Department),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
          };

            du.Execute("Sp_Insert_Master_Department", prms);
            string message = prms[2].Value.ToString();
            TempData["Msg"] = message;
            return RedirectToAction("DepartmentList");
        }


        public IActionResult DepartmentList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_DepartmentList", null);

            List<DepartmentVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DepartmentVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Department = row["Department"].ToString()
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult DeleteDepartment(int id)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@Id", id),
              new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
        };
            du.Execute("Sp_Delete_Department", prms);
            return RedirectToAction("DepartmentList");
        }

        public IActionResult DesignationList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_DesignationList", null);

            List<DesignationVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DesignationVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Designation = row["Designation"].ToString()
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult AddDesignation(DesignationVM model)
        {
            SqlParameter[] prms =
         {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@Designation", model.Designation),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
          };

            du.Execute("Sp_Insert_MasterDesignation", prms);
            string message = prms[2].Value.ToString();
            TempData["Msg"] = message;
            return RedirectToAction("DesignationList");
        }

        [HttpPost]
        public IActionResult DeleteDesignation(int id)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@Id", id),
              new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
        };

            du.Execute("Sp_Delete_Designation", prms);
            return RedirectToAction("DesignationList");
        }


        //HOLIday

        public IActionResult AddHoliday(HolidayVM model)
        {
            SqlParameter[] prms =
        {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@Holiday", model.Holiday),
        new SqlParameter("@Date", model.Date),
        new SqlParameter("@Description", model.Description),
        new SqlParameter("@Active", model.Active),
        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
          };

            du.Execute("Sp_Insert_MasterHoliday", prms);
            string message = prms[2].Value.ToString();
            TempData["Msg"] = message;
            return RedirectToAction("HolidayList");
        }


        public IActionResult HolidayList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_HolidayList", null);

            List<HolidayVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new HolidayVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Holiday= row["Holiday"].ToString(),
                    Date = row["Date"].ToString(),
                    Description = row["Description"].ToString(),
                    Active = row["Active"].ToString(),
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult DeleteHoliday(int id)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@Id", id),
              new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
        };
            du.Execute("Sp_Delete_Holiday", prms);
            return RedirectToAction("HolidayList");
        }
    }
}

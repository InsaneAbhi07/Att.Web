using System.Data;
using System.Diagnostics;
using ATPL_Attendance_SW.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
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

        public IActionResult HolidayList()
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

            return View(list);
        }

        // INSERT / UPDATE
        [HttpPost]
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

            TempData["Msg"] = prms[5].Value?.ToString();

            return RedirectToAction("HolidayList");
        }

        // DELETE
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


        private List<SelectListItem> GetDepartmentDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterDepartment",null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["Department"].ToString()
                });
            }
            return list;
        }

        private List<SelectListItem> GetDesignationDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterDesignation", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["Designation"].ToString()
                });
            }
            return list;
        }

        public IActionResult AddEmployee()
        {
            return View();
        }
        public IActionResult EmployeeList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_EmployeeList", null);

            List<EmployeeVM> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new EmployeeVM
                {
                    Emp_Id = Convert.ToInt32(row["Emp_Id"]),
                    Name = row["Name"].ToString(),

                    DepartmentId = Convert.ToInt32(row["DepartmentId"]),  
                    DesignationId = Convert.ToInt32(row["DesignationId"]),

                    Department = row["Department"].ToString(),
                    Designation = row["Designation"].ToString(),

                    EmailId = row["EmailId"].ToString(),
                    PhoneNo = row["PhoneNo"].ToString(),
                    JoiningDate = row["JoiningDate"].ToString(),
                    Address = row["Address"].ToString(),
                    Salary = Convert.ToDecimal(row["Salary"]),
                    Status = row["Status"].ToString(),
                    Shift = row["Shift"].ToString(),
                    Emp_Img = row["Emp_Img"].ToString()
                });

            }

            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();

            return View(list);
        }

        [HttpPost]
        [HttpPost]
        public IActionResult SaveEmployee(EmployeeVM model, IFormFile EmpImage)
        {
            string imageName = model.Emp_Img; // old image (edit case)

            if (EmpImage != null && EmpImage.Length > 0)
            {
                string uploadPath = Path.Combine(
                    Directory.GetCurrentDirectory(),
                    "wwwroot/uploads/employee"
                );

                if (!Directory.Exists(uploadPath))
                    Directory.CreateDirectory(uploadPath);

                imageName = Guid.NewGuid().ToString() + Path.GetExtension(EmpImage.FileName);
                string filePath = Path.Combine(uploadPath, imageName);

                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    EmpImage.CopyTo(stream);
                }
            }

            SqlParameter[] prms =
            {
        new SqlParameter("@Emp_Id", model.Emp_Id),
        new SqlParameter("@Name", model.Name),
        new SqlParameter("@DepartmentId", model.DepartmentId),
        new SqlParameter("@DesignationId", model.DesignationId),
        new SqlParameter("@EmailId", model.EmailId),
        new SqlParameter("@PhoneNo", model.PhoneNo),
        new SqlParameter("@JoiningDate", model.JoiningDate),
        new SqlParameter("@Address", model.Address),
        new SqlParameter("@Emp_Img", imageName),
        new SqlParameter("@Salary", model.Salary),
        new SqlParameter("@Status", model.Status),
        new SqlParameter("@Shift", model.Shift),
        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Insert_MasterEmployee", prms);
            TempData["Msg"] = prms[10].Value.ToString();

            return RedirectToAction("EmployeeList");
        }


        [HttpPost]
        public IActionResult DeleteEmployee(long empId)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@Emp_Id", empId)
    };

            du.Execute("Sp_Delete_MasterEmployee", prms);
            TempData["Msg"] = "Employee Deleted";

            return RedirectToAction("EmployeeList");
        }


    }
}

using System.Data;
using System.Diagnostics;
using ATPL_Attendance_SW.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Data.SqlClient;

namespace ATPL_Attendance_SW.Web.Controllers
{
    [Authorize(AuthenticationSchemes = "AttendanceCookie")]

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

            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();

            ViewBag.TotalHolidays = du.ExecuteScalar("SELECT COUNT(*) FROM Tbl_MasterHolidays where Active='1'");

            DataTable dt = du.GetDataTable("Sp_Get_UpcomingBirthdays",new SqlParameter[] { });
            List<EmployeeVM> birthdayList = new List<EmployeeVM>();

            foreach (DataRow row in dt.Rows)
            {
                birthdayList.Add(new EmployeeVM
                {
                    Emp_Id = Convert.ToInt32(row["Emp_Id"]),
                    Name = row["Name"].ToString(),
                    DOB = row["Dob"].ToString(),
                    PhoneNo= row["PhoneNo"].ToString()
                });
            }

            ViewBag.BirthdayList = birthdayList;

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
            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
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
                    Name = row["Name"]?.ToString(),

                    DepartmentId = row["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(row["DepartmentId"]),
                    DesignationId = row["DesignationId"] == DBNull.Value ? 0 : Convert.ToInt32(row["DesignationId"]),

                    EmailId = row["EmailId"] == DBNull.Value ? "" : row["EmailId"].ToString(),
                    PhoneNo = row["PhoneNo"] == DBNull.Value ? "" : row["PhoneNo"].ToString(),

                    JoiningDate = row["JoiningDate"] == DBNull.Value ? "" : row["JoiningDate"].ToString(),
                    Address = row["Address"] == DBNull.Value ? "" : row["Address"].ToString(),

                    Emp_Img = row["Emp_Img"] == DBNull.Value ? "" : row["Emp_Img"].ToString(),
                    Designation = row["Designation"] == DBNull.Value ? "" : row["Designation"].ToString(),
                    Department = row["Department"] == DBNull.Value ? "" : row["Department"].ToString(),

                    Salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]),
                    Status = row["Status"] == DBNull.Value ? "Inactive" : row["Status"].ToString(),
                    Shift = row["Shift"] == DBNull.Value ? "" : row["Shift"].ToString(),

                    UserName = row["UserName"] == DBNull.Value ? "" : row["UserName"].ToString(),
                    Role = row["Role"] == DBNull.Value ? "" : row["Role"].ToString()
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
            string imageName = model.Emp_Img ?? "";   // old image safe

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

                using var stream = new FileStream(filePath, FileMode.Create);
                EmpImage.CopyTo(stream);
            }

            SqlParameter[] prms =
            {
        new SqlParameter("@Emp_Id", model.Emp_Id),

        new SqlParameter("@Name", model.Name ?? ""),
        new SqlParameter("@Dob", string.IsNullOrEmpty(model.DOB) ? (object)DBNull.Value : model.DOB),

        new SqlParameter("@DepartmentId", model.DepartmentId == 0 ? (object)DBNull.Value : model.DepartmentId),
        new SqlParameter("@DesignationId", model.DesignationId == 0 ? (object)DBNull.Value : model.DesignationId),

        new SqlParameter("@EmailId", model.EmailId ?? ""),
        new SqlParameter("@PhoneNo", model.PhoneNo ?? ""),

        new SqlParameter("@JoiningDate",
            string.IsNullOrEmpty(model.JoiningDate) ? (object)DBNull.Value : model.JoiningDate),

        new SqlParameter("@Address", model.Address ?? ""),
        new SqlParameter("@Emp_Img", imageName),

        new SqlParameter("@Salary",
            model.Salary == 0 ? (object)DBNull.Value : model.Salary),

        new SqlParameter("@Status", model.Status ?? ""),
        new SqlParameter("@Shift", model.Shift ?? ""),

        new SqlParameter("@UserName", model.UserName ?? ""),
        new SqlParameter("@Password", model.Password ?? ""),
        new SqlParameter("@Role", model.Role ?? ""),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Insert_MasterEmployee", prms);

            TempData["Msg"] = prms[^1].Value?.ToString();
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


        private List<SelectListItem> GetCompanyDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_CompanyInformation", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["CCode"].ToString()
                });
            }
            return list;
        }

        public IActionResult CompanyInfoList()
        {
            ViewBag.CCodeList = GetCompanyDDL();

            DataTable dt = du.GetDataTable("Sp_Get_CompanyList", null);
            List<CompanyVM> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new CompanyVM
                {
                    Id = Convert.ToInt64(r["Id"]),
                    CCode = r["CCode"].ToString(),
                    CompanyName = r["CompanyName"].ToString(),
                    Abbr = r["Abbr"].ToString(),
                    Address = r["Address"].ToString(),
                    Address2 = r["Address2"].ToString(),
                    City = r["City"].ToString(),
                    State = r["State"].ToString(),
                    Pincode = r["Pincode"].ToString(),
                    Country = r["Country"].ToString(),
                    Telepohne = r["Telepohne"].ToString(),
                    ContactNo = r["ContactNo"].ToString(),
                    Email = r["Email"].ToString(),
                    Website = r["Website"].ToString(),
                    GSTIN = r["GSTIN"].ToString(),
                    BankName = r["BankName"].ToString(),
                    AccountNo = r["AccountNo"].ToString(),
                    IFSCCode = r["IFSCCode"].ToString()


                });
            }

            return View(list);
        }


        [HttpPost]
        public IActionResult SaveCompany(CompanyVM model)
        {
            SqlParameter[] prms =
      {
            new SqlParameter("@Id", model.Id),
            new SqlParameter("@CCode", model.CCode ?? ""),
            new SqlParameter("@CompanyName", model.CompanyName),
            new SqlParameter("@Abbr", model.Abbr),
            new SqlParameter("@Address", model.Address),
            new SqlParameter("@Address2", model.Address2),
            new SqlParameter("@City", model.City),
            new SqlParameter("@State", model.State),
            new SqlParameter("@Pincode", model.Pincode),
            new SqlParameter("@Country", model.Country),
            new SqlParameter("@Telepohne", model.Telepohne),
            new SqlParameter("@ContactNo", model.ContactNo),
            new SqlParameter("@Email", model.Email),
            new SqlParameter("@Website", model.Website),
            new SqlParameter("@GSTIN", model.GSTIN),
            new SqlParameter("@BankName", model.BankName),
            new SqlParameter("@AccountNo", model.AccountNo),
            new SqlParameter("@IFSCCode", model.IFSCCode),
            new SqlParameter("@msg", SqlDbType.NVarChar, 100)
            { Direction = ParameterDirection.Output }
        };

            du.Execute("Sp_Insert_CompanyInformation", prms);
            TempData["Msg"] = prms[^1].Value.ToString();

            return RedirectToAction("CompanyInfoList");
        }

        [HttpGet]
        public JsonResult GetCompanyByCode(string ccode)
        {
            if (string.IsNullOrEmpty(ccode)) 
                return Json(null);

            SqlParameter[] prms =
            {
            new SqlParameter("@CCode", ccode)
    };

            DataTable dt = du.GetDataTable("Sp_Get_CompanyByCode", prms);

            if (dt.Rows.Count == 0)
                return Json(null);

            var r = dt.Rows[0];

            var data = new
            {
                Id = r["Id"].ToString(),
                CompanyName = r["CompanyName"].ToString(),
                Abbr = r["Abbr"].ToString(),
                Address = r["Address"].ToString(),
                Address2 = r["Address2"].ToString(),
                City = r["City"].ToString(),
                State = r["State"].ToString(),
                Pincode = r["Pincode"].ToString(),
                Country = r["Country"].ToString(),
                Telepohne = r["Telepohne"].ToString(),
                ContactNo = r["ContactNo"].ToString(),
                Email = r["Email"].ToString(),
                Website = r["Website"].ToString(),
                GSTIN = r["GSTIN"].ToString(),
                BankName = r["BankName"].ToString(),
                AccountNo = r["AccountNo"].ToString(),
                IFSCCode = r["IFSCCode"].ToString()
            };

            return Json(data);
        }


        [HttpPost]
        public IActionResult DeleteCompany(long id)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@Id", id)
        };

            du.Execute("Sp_Delete_MasterCompany", prms);
            TempData["Msg"] = "Record Deleted Successfully";

            return RedirectToAction("CompanyList");
        }



        public IActionResult ActiveEmployeeDashboard()
        {
            ViewBag.DesignationList = GetDesignationDDL();

            var list = GetEmployeeList(null, null);
            return View(list);
        }


        [HttpGet]
        public IActionResult FilterEmployeeList(long? designationId, string status)
        {
            var list = GetEmployeeList(designationId, status);
            return PartialView("_EmployeeDashboardRows", list ?? new List<EmployeeVM>());
        }


        private List<EmployeeVM> GetEmployeeList(long? desigId, string status)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@DesignationId", (object?)desigId ?? DBNull.Value),
        new SqlParameter("@Status", string.IsNullOrEmpty(status) ? DBNull.Value : status)
    };

            DataTable dt = du.GetDataTable("Sp_Get_DashboardEmployeeList", prms);

            List<EmployeeVM> list = new();

            if (dt == null || dt.Rows.Count == 0)
                return list;

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new EmployeeVM
                {
                    Emp_Id = Convert.ToInt32(r["Emp_Id"]),
                    Name = r["Name"]?.ToString(),
                    EmailId = r["EmailId"]?.ToString(),
                    PhoneNo = r["PhoneNo"]?.ToString(),
                    Designation = r["Designation"]?.ToString(),
                    Department= r["Department"]?.ToString(),
                    JoiningDate = r["JoiningDate"]?.ToString(),
                    Status = r["Status"]?.ToString(),
                    Emp_Img = r["Emp_Img"]?.ToString(),
                    Address = r["Address"]?.ToString()
                });
            }

            return list;
        }


    }
}

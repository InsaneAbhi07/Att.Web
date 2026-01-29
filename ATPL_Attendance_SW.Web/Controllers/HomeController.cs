using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Reflection.Emit;
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
            ViewBag.Shiftlistt = GetSHiftDDL();

            ViewBag.TotalHolidays = du.ExecuteScalar("SELECT COUNT(*) FROM Tbl_MasterHolidays where Active='1'");
            ViewBag.ActiveE = du.ExecuteScalar("SELECT COUNT(*) FROM Tbl_MasterEmployeeDetails where Status='Active'");
            ViewBag.TotalR_E = du.ExecuteScalar("SELECT COUNT(*) FROM Tbl_MasterEmployeeDetails");

            DataTable dt = du.GetDataTable("Sp_Get_UpcomingBirthdays", new SqlParameter[] { });
            List<EmployeeVM> birthdayList = new List<EmployeeVM>();

            foreach (DataRow row in dt.Rows)
            {
                birthdayList.Add(new EmployeeVM
                {
                    Emp_Id = Convert.ToInt32(row["Emp_Id"]),
                    Name = row["Name"].ToString(),
                    DOB = row["Dob"].ToString(),
                    PhoneNo = row["PhoneNo"].ToString()
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
            ViewBag.DepartmentList = GetDepartmentDDL();
            DataTable dt = du.GetDataTable("Sp_Get_DesignationList", null);

            List<DesignationVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DesignationVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Designation = row["Designation"].ToString(),
                    Department = row["Department"].ToString(),
                    DepartmentId = Convert.ToInt32(row["DepartmentId"])

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
        new SqlParameter("@DepartmentId", model.DepartmentId),
        new SqlParameter("@Designation", model.Designation),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
          };

            du.Execute("Sp_Insert_MasterDesignation", prms);
            string message = prms[3].Value.ToString();
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
                    Date = Convert.ToDateTime(row["Date"].ToString()),
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
            new SqlParameter("@Date", SqlDbType.Date)
            {
                Value = model.Date
            },
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
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterDepartment", null);
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

        private List<SelectListItem> GetBranchDDL1()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterBranch", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["BranchName"].ToString()
                });
            }
            return list;
        }

        private List<SelectListItem> GetWorkUnderDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select Emp_Id,Name From Tbl_MasterEmployeeDetails", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Emp_Id"].ToString(),
                    Text = row["Name"].ToString()
                });
            }
            return list;
        }

        private List<SelectListItem> GetEmployeeTypeDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select Id,EmployeeType From Tbl_MasterEmployeeType", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["EmployeeType"].ToString()
                });
            }
            return list;
        }

        [HttpGet]
        public JsonResult GetDesignationsByDepartment(int departmentId)
        {
            string query = "SELECT Id, Designation FROM Tbl_MasterDesignation WHERE DepartmentId = @DepartmentId";
            SqlParameter[] prms = { new SqlParameter("@DepartmentId", departmentId) };
            DataTable dt = du.GetDataTableByQuery(query, prms);

            List<object> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new
                {
                    value = row["Id"].ToString(),
                    text = row["Designation"].ToString()
                });
            }
            return Json(list);
        }


        private List<SelectListItem> GetSHiftDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterShift", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["Shift"].ToString()
                });
            }
            return list;
        }

        public IActionResult AddEmployee()
        {
            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
            ViewBag.Shiftlistt = GetSHiftDDL();
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
                    BranchId = row["BranchId"] == DBNull.Value ? 0 : Convert.ToInt32(row["BranchId"]),
                    ShiftId = row["ShiftId"] == DBNull.Value ? 0 : Convert.ToInt32(row["ShiftId"]),

                    EmailId = row["EmailId"] == DBNull.Value ? "" : row["EmailId"].ToString(),
                    PhoneNo = row["PhoneNo"] == DBNull.Value ? "" : row["PhoneNo"].ToString(),

                    JoiningDate = row["JoiningDate"] == DBNull.Value ? "" : row["JoiningDate"].ToString(),
                    Address = row["Address"] == DBNull.Value ? "" : row["Address"].ToString(),
                    DOB = row["Dob"] == DBNull.Value ? "" : row["Dob"].ToString(),

                    Emp_Img = row["Emp_Img"] == DBNull.Value ? "" : row["Emp_Img"].ToString(),
                    Designation = row["Designation"] == DBNull.Value ? "" : row["Designation"].ToString(),
                    Department = row["Department"] == DBNull.Value ? "" : row["Department"].ToString(),
                    BranchName = row["BranchName"] == DBNull.Value ? "" : row["BranchName"].ToString(),

                    Salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]),
                    Status = row["Status"] == DBNull.Value ? "Inactive" : row["Status"].ToString(),
                    Shift = row["Shift"] == DBNull.Value ? "" : row["Shift"].ToString(),

                    BankName = row["BankName"] == DBNull.Value ? "" : row["Shift"].ToString(),
                    Ac_HolderName = row["Ac_HolderName"] == DBNull.Value ? "" : row["Shift"].ToString(),
                    IFSC_Code = row["IFSC_Code"] == DBNull.Value ? "" : row["Shift"].ToString(),
                    Acc_No = row["Acc_No"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Acc_No"]),
                    EmpTypeId = row["EmpTypeId"] == DBNull.Value ? 0 : Convert.ToInt32(row["EmpTypeId"]),
                    EmployeeType = row["EmployeeType"] == DBNull.Value ? "" : row["EmployeeType"].ToString(),


                    UserName = row["UserName"] == DBNull.Value ? "" : row["UserName"].ToString(),
                    Role = row["Role"] == DBNull.Value ? "" : row["Role"].ToString()
                });


            }

            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
            ViewBag.Shiftlistt = GetSHiftDDL();
            ViewBag.BranchList = GetBranchDDL1();

            return View(list);
        }

         public IActionResult EmployeeList1()
        {
            DataTable dt = du.GetDataTable("Sp_Get_EmployeeList", null);
            List<EmployeeVM> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new EmployeeVM
                {
                    Emp_Id = Convert.ToInt32(row["Emp_Id"]),
                    Name = row["Name"]?.ToString(),
                    Emp_Code = row["Emp_Code"]?.ToString(),
                    DepartmentId = row["DepartmentId"] == DBNull.Value ? 0 : Convert.ToInt32(row["DepartmentId"]),
                    DesignationId = row["DesignationId"] == DBNull.Value ? 0 : Convert.ToInt32(row["DesignationId"]),
                    BranchId = row["BranchId"] == DBNull.Value ? 0 : Convert.ToInt32(row["BranchId"]),
                    ShiftId = row["ShiftId"] == DBNull.Value ? 0 : Convert.ToInt32(row["ShiftId"]),

                    EmailId = row["EmailId"]?.ToString() ?? "",
                    PhoneNo = row["PhoneNo"]?.ToString() ?? "",

                    JoiningDate = row["JoiningDate"]?.ToString() ?? "",
                    Address = row["Address"]?.ToString() ?? "",
                    DOB = row["Dob"]?.ToString() ?? "",

                    Emp_Img = row["Emp_Img"]?.ToString() ?? "",
                    Designation = row["Designation"]?.ToString() ?? "",
                    Department = row["Department"]?.ToString() ?? "",
                    EmployeeType = row["EmployeeType"]?.ToString() ?? "",
                    BranchName = row["BranchName"]?.ToString() ?? "",

                    Salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]),
                    Status = row["Status"]?.ToString() ?? "Inactive",
                    Shift = row["Shift"]?.ToString() ?? "",

                    BankName = row["BankName"]?.ToString() ?? "",
                    Ac_HolderName = row["AC_HolderName"]?.ToString() ?? "",
                    IFSC_Code = row["IFSC_Code"]?.ToString() ?? "",
                    Acc_No = row["Acc_No"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Acc_No"]),
                    EmpTypeId = row["EmpTypeId"] == DBNull.Value ? 0 : Convert.ToInt32(row["EmpTypeId"]),
                    UserName = row["UserName"]?.ToString() ?? "",
                    Role = row["Role"]?.ToString() ?? ""
                });
            }

            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
            ViewBag.Shiftlistt = GetSHiftDDL();
            ViewBag.EmployeeType = GetEmployeeTypeDDL();
            ViewBag.BranchList = GetBranchDDL1();

            return View(list);
        }

        public IActionResult SaveEmployee(long id = 0)
        {
            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
            ViewBag.Shiftlistt = GetSHiftDDL();
            ViewBag.WorkUnderList = GetWorkUnderDDL();
            ViewBag.EmployeeType = GetEmployeeTypeDDL();
            ViewBag.BranchList = GetBranchDDL1();
            // ADD
            if (id == 0)
                return View(new EmployeeVM());

            // EDIT
            SqlParameter[] p = { new SqlParameter("@EmpId", id) };
            DataTable dt = du.GetDataTable("Sp_GetEmployeeById", p);

            if (dt.Rows.Count == 0)
                return RedirectToAction("EmployeeList");

            DataRow r = dt.Rows[0];

            EmployeeVM model = new EmployeeVM
            {
                Emp_Id = (int)id,
                Name = r["Name"]?.ToString(),
                DOB = r["DOB"]?.ToString(),
                JoiningDate = r["JoiningDate"]?.ToString(),
                DepartmentId = Convert.ToInt32(r["DepartmentId"]),
                DesignationId = Convert.ToInt32(r["DesignationId"]),
                BranchId = Convert.ToInt32(r["BranchId"]),
                ShiftId = Convert.ToInt32(r["ShiftId"]),
                EmailId = r["EmailId"]?.ToString(),
                PhoneNo = r["PhoneNo"]?.ToString(),
                Address = r["Address"]?.ToString(),
                Emp_Img = r["Emp_Img"]?.ToString(),
                Salary = r["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Salary"]),
                Status = r["Status"]?.ToString(),
                UserName = r["UserName"]?.ToString(),
                Password = r["PasswordHash"]?.ToString(),
                BankName = r["BankName"]?.ToString(),
                Ac_HolderName = r["AC_HolderName"]?.ToString(),
                IFSC_Code = r["IFSC_Code"]?.ToString(),
                Acc_No = r["Acc_No"] == DBNull.Value ? 0 : Convert.ToDecimal(r["Acc_No"]),
                EmpTypeId = Convert.ToInt32(r["EmpTypeId"]),


                Gender = r["Gender"]?.ToString(),
                MaritalStatus = r["MaritalStatus"]?.ToString(),
                EmergencyContact = r["EmergencyContact"]?.ToString(),
                BasicSalary = r["BasicSalary"] == DBNull.Value ? 0 : Convert.ToDecimal(r["BasicSalary"]),
                HRA = r["HRA"] == DBNull.Value ? 0 : Convert.ToDecimal(r["HRA"]),
                DA = r["DA"] == DBNull.Value ? 0 : Convert.ToDecimal(r["DA"]),
                PA = r["PA"] == DBNull.Value ? 0 : Convert.ToDecimal(r["PA"]),
                OtherAllowance = r["OtherAllowance"] == DBNull.Value ? 0 : Convert.ToDecimal(r["OtherAllowance"]),
                GrossSalary = r["GrossSalary"] == DBNull.Value ? 0 : Convert.ToDecimal(r["GrossSalary"]),
                Aadhardoc = r["Aadhardoc"]?.ToString(),
                ResumeDoc = r["ResumeDoc"]?.ToString(),
                Emp_Code = r["Emp_Code"]?.ToString(),
                WorkUnderId = Convert.ToInt32(r["DesignationId"]),
            };
            return View(model);
        }
        [HttpPost]
        public IActionResult SaveEmployee(EmployeeVM model, IFormFile EmpImage, IFormFile ResumeDocument, IFormFile AadharDocument)
        {
            string img = model.Emp_Img; // Existing image if editing
            string resume = model.ResumeDoc; // Existing resume if editing
            string aadhar = model.Aadhardoc; // Existing aadhar if editing

            // Handle Employee Image Upload
            if (EmpImage != null && EmpImage.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "employee");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                // Generate unique filename
                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(EmpImage.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                // Save file
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    EmpImage.CopyTo(fileStream);
                }

                img = uniqueFileName;
            }

            if (ResumeDocument != null && ResumeDocument.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ResumeDocument.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    ResumeDocument.CopyTo(fileStream);
                }

                resume = uniqueFileName;
            }

            if (AadharDocument != null && AadharDocument.Length > 0)
            {
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "documents");
                if (!Directory.Exists(uploadsFolder))
                {
                    Directory.CreateDirectory(uploadsFolder);
                }

                string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(AadharDocument.FileName);
                string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    AadharDocument.CopyTo(fileStream);
                }

                aadhar = uniqueFileName;
            }

            SqlParameter[] prms =
            {
        new SqlParameter("@Emp_Id", model.Emp_Id),
        new SqlParameter("@Emp_Code", model.Emp_Code),
        new SqlParameter("@Name", model.Name),
        new SqlParameter("@DOB", model.DOB ?? (object)DBNull.Value),
        new SqlParameter("@DepartmentId", model.DepartmentId),
        new SqlParameter("@DesignationId", model.DesignationId),
        new SqlParameter("@BranchId", model.BranchId),
        new SqlParameter("@ShiftId", model.ShiftId),
        new SqlParameter("@JoiningDate", model.JoiningDate ?? (object)DBNull.Value),
        new SqlParameter("@ResignDate", model.ResignDate ?? (object)DBNull.Value),
        new SqlParameter("@PhoneNo", model.PhoneNo),
        new SqlParameter("@EmailId", model.EmailId ?? (object)DBNull.Value),
        new SqlParameter("@Address", model.Address ?? (object)DBNull.Value),
        new SqlParameter("@Gender", model.Gender ?? (object)DBNull.Value),
        new SqlParameter("@MaritalStatus", model.MaritalStatus ?? (object)DBNull.Value),
        new SqlParameter("@EmergencyContact", model.EmergencyContact ?? (object)DBNull.Value),
        new SqlParameter("@EmpTypeId", model.EmpTypeId),
        new SqlParameter("@Status", model.Status),
        new SqlParameter("@UserName", model.UserName ?? (object)DBNull.Value),
        new SqlParameter("@Password", model.Password ?? (object)DBNull.Value),
        new SqlParameter("@Role", model.Role ?? (object)DBNull.Value),
        new SqlParameter("@BankName", model.BankName ?? (object)DBNull.Value),
        new SqlParameter("@AC_HolderName", model.Ac_HolderName ?? (object)DBNull.Value),
        new SqlParameter("@IFSC_Code", model.IFSC_Code ?? (object)DBNull.Value),
        new SqlParameter("@Acc_No", model.Acc_No ?? (object)DBNull.Value),
        new SqlParameter("@BasicSalary", model.BasicSalary),
        new SqlParameter("@HRA", model.HRA),
        new SqlParameter("@DA", model.DA),
        new SqlParameter("@PA", model.PA),
        new SqlParameter("@OtherAllowance", model.OtherAllowance),
        new SqlParameter("@GrossSalary", model.GrossSalary),
        new SqlParameter("@WorkUnderId", model.WorkUnderId),
        new SqlParameter("@Emp_Img", img ?? (object)DBNull.Value),
        new SqlParameter("@ResumeDoc", resume ?? (object)DBNull.Value),
        new SqlParameter("@AadharDoc", aadhar ?? (object)DBNull.Value),
        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Insert_MasterEmployee", prms);
            TempData["Msg"] = prms[^1].Value.ToString();
            return RedirectToAction("EmployeeList1");
        }

        [HttpGet]
        public IActionResult CheckEmpCode(string empCode, int empId = 0)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@EmpCode", empCode),
        new SqlParameter("@EmpId", empId)
    };

            DataTable dt = du.GetDataTableByQuery("SELECT 1 FROM Tbl_MasterEmployeeDetails WHERE Emp_Code = " +
                "@EmpCode AND Emp_Id<>@EmpId", prms);

            return Json(dt.Rows.Count > 0);
        }


        [HttpGet]
        public IActionResult CheckUsername(string username, string empId)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@username", username),
        new SqlParameter("@EmpId", empId)
    };

            DataTable dt = du.GetDataTableByQuery("SELECT 1 FROM Tbl_MasterEmployeeDetails WHERE UserName = " +
                "@username AND Emp_Id<>@EmpId", prms);

            return Json(dt.Rows.Count > 0);
        }

        [HttpPost]
        public IActionResult DeleteEmployee(string empCode)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@Emp_Id", empCode)
    };

            du.Execute("Sp_Delete_MasterEmployee", prms);
            TempData["Msg"] = "Employee Deleted";

            return RedirectToAction("EmployeeList1");
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


        private List<SelectListItem> GetStatesDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterState", null);
            List<SelectListItem> state = new();
            foreach (DataRow row in dt.Rows)
            {
                state.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["State"].ToString()
                });
            }
            return state;
        }
        private List<SelectListItem> GetCityDDL(long stateId)
        {
            var param = new SqlParameter[]
            {
                 new SqlParameter("@StateId", stateId)
            };

            DataTable dt = du.GetDataTableByQuery("SELECT Id, City FROM Tbl_MasterCity WHERE StateId = @StateId", param);
            List<SelectListItem> city = new();
            foreach (DataRow row in dt.Rows)
            {
                city.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["City"].ToString()
                });
            }
            return city;
        }

        private List<SelectListItem> GetBranchTypeDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterBranchType", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["BranchType"].ToString()
                });
            }
            return list;
        }
        public IActionResult LoadCities(long stateId)
        {
            var data = GetCityDDL(stateId);
            return Json(data);
        }

        public IActionResult LoadStates()
        {
            var data = GetStatesDDL();
            return Json(data);
        }


        public IActionResult CompanyInfoList()
        {
            ViewBag.CCodeList = GetCompanyDDL();
            ViewBag.StateList = GetStatesDDL();
            ViewBag.CityList = new List<SelectListItem>();


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
                    IFSCCode = r["IFSCCode"].ToString(),
                    LicenseNo = r["LicenseNo"].ToString(),
                    PANNo = r["PANNo"].ToString(),
                    UANNo = r["UANNo"].ToString()


                });
            }

            return View(list);
        }


        [HttpPost]
        public IActionResult SaveCompany(CompanyVM model)
        {
            if (!ModelState.IsValid)
            {
                // Dropdown data wapas bharo
                ViewBag.StateList = GetStatesDDL();
                return View("CompanyInfoList", GetCompanyDDL());
            }

            SqlParameter[] prms =
            {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@CCode", model.CCode ?? ""),
        new SqlParameter("@CompanyName", model.CompanyName ?? ""),
        new SqlParameter("@Abbr", model.Abbr ?? ""),

        new SqlParameter("@Address", model.Address ?? ""),
        new SqlParameter("@Address2", model.Address2 ?? ""),

        new SqlParameter("@StateId", model.StateId ?? (object)DBNull.Value),
        new SqlParameter("@CityId", model.CityId ?? (object)DBNull.Value),

        new SqlParameter("@Pincode", model.Pincode ?? ""),
        new SqlParameter("@Country", model.Country ?? ""),

        new SqlParameter("@Telepohne", model.Telepohne ?? (object)DBNull.Value),
        new SqlParameter("@ContactNo", model.ContactNo ??(object)DBNull.Value),
        new SqlParameter("@Email", model.Email ?? ""),
        new SqlParameter("@Website", model.Website ?? ""),
        new SqlParameter("@GSTIN", model.GSTIN ?? ""),

        new SqlParameter("@BankName", model.BankName ?? ""),
        new SqlParameter("@AccountNo", model.AccountNo ?? ""),
        new SqlParameter("@IFSCCode", model.IFSCCode ?? ""),
        new SqlParameter("@LicenseNo", model.LicenseNo ?? ""),
        new SqlParameter("@PANNo", model.PANNo ?? ""),
        new SqlParameter("@UANNo", model.UANNo ?? ""),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Insert_CompanyInformation", prms);

            TempData["Msg"] = prms[^1].Value?.ToString();
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
                ccode = r["CCode"].ToString(),
                CompanyName = r["CompanyName"].ToString(),
                Abbr = r["Abbr"].ToString(),
                Address = r["Address"].ToString(),
                Address2 = r["Address2"].ToString(),
                CityId = r["CityId"].ToString(),
                StateId = r["StateId"].ToString(),
                Pincode = r["Pincode"].ToString(),
                Country = r["Country"].ToString(),
                Telepohne = r["Telepohne"].ToString(),
                ContactNo = r["ContactNo"].ToString(),
                Email = r["Email"].ToString(),
                Website = r["Website"].ToString(),
                GSTIN = r["GSTIN"].ToString(),
                BankName = r["BankName"].ToString(),
                AccountNo = r["AccountNo"].ToString(),
                IFSCCode = r["IFSCCode"].ToString(),
                LicenseNo = r["LicenseNo"].ToString(),
                PANNo = r["PANNo"].ToString(),
                UANNo = r["UANNo"].ToString()
            };

            return Json(data);
        }


        [HttpPost]
        public IActionResult DeleteCompany(string ccode)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@CCode", ccode)
        };

            du.Execute("Sp_Delete_MasterCompany", prms);
            TempData["Msg"] = "Record Deleted Successfully";

            return RedirectToAction("CompanyInfoList");
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
                    Department = r["Department"]?.ToString(),
                    JoiningDate = r["JoiningDate"]?.ToString(),
                    Status = r["Status"]?.ToString(),
                    Emp_Img = r["Emp_Img"]?.ToString(),
                    Address = r["Address"]?.ToString()
                });
            }

            return list;
        }

        public IActionResult ShiftList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_MasterShift", null);

            List<MasterShift> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new MasterShift
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Shift = row["Shift"].ToString(),
                    ShiftType = row["ShiftType"].ToString(),
                    InTime = row["InTime"].ToString(),
                    OutTime = row["OutTime"].ToString(),
                    BreakOut = row["BreakOut"].ToString(),
                    BreakIn = row["BreakIn"].ToString(),
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult AddShift(MasterShift model)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@Id", model.Id),

        new SqlParameter("@Shift",
            string.IsNullOrWhiteSpace(model.Shift) ? (object)DBNull.Value : model.Shift),

        new SqlParameter("@ShiftType",
            string.IsNullOrWhiteSpace(model.ShiftType) ? (object)DBNull.Value : model.ShiftType),

        new SqlParameter("@InTime",
            string.IsNullOrWhiteSpace(model.InTime) ? (object)DBNull.Value : model.InTime),

        new SqlParameter("@OutTime",
            string.IsNullOrWhiteSpace(model.OutTime) ? (object)DBNull.Value : model.OutTime),

        new SqlParameter("@BreakOut",
            string.IsNullOrWhiteSpace(model.BreakOut) ? (object)DBNull.Value : model.BreakOut),

        new SqlParameter("@BreakIn",
            string.IsNullOrWhiteSpace(model.BreakIn) ? (object)DBNull.Value : model.BreakIn),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Save_MasterShift", prms);

            TempData["Msg"] = prms[7].Value?.ToString();

            return RedirectToAction("ShiftList");
        }


        [HttpPost]
        public IActionResult DeleteShift(int id)
        {
            SqlParameter[] prms =
            {
                  new SqlParameter("@Id", id),
                  new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
             };

            du.Execute("Sp_Delete_MasterShift", prms);
            return RedirectToAction("ShiftList");
        }

        private static List<LeaveTypeVM> _leaveTypes = new();

        // LIST
        public IActionResult LeaveTypeList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_LeaveTypeList", null);

            List<LeaveTypeVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new LeaveTypeVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    LeaveType = row["LeaveType"].ToString(),
                    Penalty = Convert.ToInt32(row["Penalty"]),
                    Total_Y = Convert.ToInt32(row["Total_Y"]),
                    CarryForward = row["CarryForward"].ToString(),
                    Compensationin = row["Compensationin"].ToString(),
                });
            }
            return View(list);
        }

        // ADD / UPDATE
        [HttpPost]
        public IActionResult AddLeaveType(LeaveTypeVM model)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@Id", model.Id),

        new SqlParameter("@LeaveType",
            string.IsNullOrWhiteSpace(model.LeaveType)
                ? (object)DBNull.Value
                : model.LeaveType),

        new SqlParameter("@Penalty", model.Penalty),
        new SqlParameter("@Total_Y", model.Total_Y),

        new SqlParameter("@CarryForward",
            string.IsNullOrWhiteSpace(model.CarryForward)
                ? (object)DBNull.Value
                : model.CarryForward),

        new SqlParameter("@Compensationin",
            string.IsNullOrWhiteSpace(model.Compensationin)
                ? (object)DBNull.Value
                : model.Compensationin),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Insert_Master_LeaveType", prms);

            TempData["Msg"] = prms[6].Value.ToString();
            return RedirectToAction("LeaveTypeList");
        }


        // DELETE
        [HttpPost]
        public IActionResult DeleteLeaveType(int id)
        {
            SqlParameter[] prms =
         {
                  new SqlParameter("@Id", id),
                  new SqlParameter("@msg", SqlDbType.NVarChar, 100)
                {
                    Direction = ParameterDirection.Output
                }
         };

            du.Execute("Sp_Delete_LeaveType", prms);
            return RedirectToAction("LeaveTypeList");
        }
        public IActionResult MasterStateCityList()
        {

            DataTable dtState = du.GetDataTable("Sp_Get_MasterState", null);

            List<MasterStateVM> states = new();
            foreach (DataRow row in dtState.Rows)
            {
                states.Add(new MasterStateVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    State = row["State"].ToString(),
                });
            }

            DataTable dtCity = du.GetDataTable("Sp_Get_MasterCity", null);
            List<MasterCityVM> cities = new();
            foreach (DataRow row in dtCity.Rows)
            {
                cities.Add(new MasterCityVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    StateId = Convert.ToInt32(row["StateId"]),
                    City = row["City"].ToString()
                });
            }
            ViewBag.States = states;
            ViewBag.Cities = cities;

            return View();
        }

        [HttpPost]
        public IActionResult SaveState(MasterStateVM model)
        {
            SqlParameter[] prms = { new SqlParameter("@Id", model.Id),

                new SqlParameter("@State",string.IsNullOrWhiteSpace(model.State)? (object)DBNull.Value: model.State),
                new SqlParameter("@msg", SqlDbType.NVarChar, 100)
                {
                    Direction = ParameterDirection.Output
                }
            };
            du.Execute("Sp_InsertMasterState", prms);
            TempData["Msg"] = prms[2].Value.ToString();
            return RedirectToAction("MasterStateCityList");

        }

        public IActionResult SaveCity(MasterCityVM model)
        {
            SqlParameter[] prms =
            {
                new SqlParameter("@Id", model.Id),
                new SqlParameter("@StateId", model.StateId),
                new SqlParameter("@City", model.City),
                new SqlParameter("@msg", SqlDbType.NVarChar,100){Direction=ParameterDirection.Output}
            };

            du.Execute("Sp_InsertMasterCity", prms);
            TempData["Msg"] = prms[3].Value.ToString();
            return RedirectToAction("MasterStateCityList");

        }

        public IActionResult DeleteState(int id)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@Id", id),
        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Delete_State", prms);

            // Capture the output message
            string message = prms[1].Value?.ToString() ?? "Operation completed";
            TempData["Msg"] = message;

            return RedirectToAction("MasterStateCityList");
        }

        public IActionResult DeleteCity(int id)
        {
            SqlParameter[] prms =
{
        new SqlParameter("@Id", id),
        new SqlParameter("@msg", SqlDbType.NVarChar,100){Direction=ParameterDirection.Output}
    };

            du.Execute("Sp_Delete_City", prms);
            TempData["Msg"] = prms[1].Value.ToString();
            return RedirectToAction("MasterStateCityList");
        }


        //BRANCHType
        public IActionResult AddBranchType(BranchTypeVM model)
        {
            SqlParameter[] prms =
        {
           new SqlParameter("@Id", model.Id),
           new SqlParameter("@BranchType", model.BranchType),

           new SqlParameter("@msg", SqlDbType.NVarChar, 100)
           {
               Direction = ParameterDirection.Output
           }
             };

            du.Execute("Sp_Insert_Master_BranchType", prms);
            string message = prms[2].Value.ToString();
            TempData["Msg"] = message;
            return RedirectToAction("BranchTypeList");
        }


        public IActionResult BranchTypeList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_BranchTypeList", null);

            List<BranchTypeVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new BranchTypeVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    BranchType = row["BranchType"].ToString()
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult DeleteBranchType(int id)
        {
            SqlParameter[] prms =
            {
       new SqlParameter("@Id", id),
         new SqlParameter("@msg", SqlDbType.NVarChar, 100)
   {
       Direction = ParameterDirection.Output
   }
   };
            du.Execute("Sp_Delete_BranchType", prms);
            return RedirectToAction("BranchTypeList");
        }


        //Document Type

        public IActionResult AddDocumentType(DocumentTypeVM model)
        {
            SqlParameter[] prms =
        {
           new SqlParameter("@Id", model.Id),
           new SqlParameter("@DocumentName", model.DocumentName),

           new SqlParameter("@msg", SqlDbType.NVarChar, 100)
           {
               Direction = ParameterDirection.Output
           }
             };

            du.Execute("Sp_Insert_Master_DocumentType", prms);
            string message = prms[2].Value.ToString();
            TempData["Msg"] = message;
            return RedirectToAction("DocumentTypeeList");
        }


        public IActionResult DocumentTypeeList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_DocumentTypeList", null);

            List<DocumentTypeVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new DocumentTypeVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    DocumentName = row["DocumentName"].ToString()
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult DeleteDocumentType(int id)
        {
            SqlParameter[] prms =
            {
       new SqlParameter("@Id", id),
         new SqlParameter("@msg", SqlDbType.NVarChar, 100)
   {
       Direction = ParameterDirection.Output
   }
   };
            du.Execute("Sp_Delete_DocumentType", prms);
            return RedirectToAction("DocumentTypeeList");
        }

        //Master Branch


        public IActionResult BranchInfoList()
        {
            ViewBag.CCodeList = GetCompanyDDL();
            ViewBag.StateList = GetStatesDDL();
            ViewBag.BranchTpyp = GetBranchTypeDDL();
            ViewBag.CityList = new List<SelectListItem>();


            DataTable dt = du.GetDataTable("Sp_Get_BranchList", null);
            List<MasterBranchVM> list = new();

            foreach (DataRow r in dt.Rows)
            {
                list.Add(new MasterBranchVM
                {
                    Id = Convert.ToInt64(r["Id"]),
                    BCode = r["BCode"].ToString(),
                    BranchName = r["BranchName"].ToString(),
                    Abbr = r["Abbr"].ToString(),
                    BranchType = r["BranchType"].ToString(),
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
                    IFSCCode = r["IFSCCode"].ToString(),
                    LicenseNo = r["LicenseNo"].ToString(),
                    PANNo = r["PANNo"].ToString(),
                    UANNo = r["UANNo"].ToString()
                });
            }

            return View(list);
        }

        private List<SelectListItem> GetBranchDDL()
        {
            DataTable dt = du.GetDataTableByQuery("Select * From Tbl_MasterBranch", null);
            List<SelectListItem> list = new();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new SelectListItem
                {
                    Value = row["Id"].ToString(),
                    Text = row["BCode"].ToString()
                });
            }
            return list;
        }

        [HttpPost]
        public IActionResult SaveBranch(MasterBranchVM model)
        {
            if (!ModelState.IsValid)
            {
                // Dropdown data wapas bharo
                ViewBag.StateList = GetStatesDDL();
                return View("BranchInfoList", GetBranchDDL());
            }

            SqlParameter[] prms =
            {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@BCode", model.BCode ?? ""),
        new SqlParameter("@BranchName", model.BranchName ?? ""),
        new SqlParameter("@Abbr", model.Abbr ?? ""),

        new SqlParameter("@BTypeId", model.BTypeId ?? (object)DBNull.Value),
        new SqlParameter("@Address", model.Address ?? ""),
        new SqlParameter("@Address2", model.Address2 ?? ""),

        new SqlParameter("@StateId", model.StateId ?? (object)DBNull.Value),
        new SqlParameter("@CityId", model.CityId ?? (object)DBNull.Value),

        new SqlParameter("@Pincode", model.Pincode ?? ""),
        new SqlParameter("@Country", model.Country ?? ""),

        new SqlParameter("@Telepohne", model.Telepohne ?? (object)DBNull.Value),
        new SqlParameter("@ContactNo", model.ContactNo ??(object)DBNull.Value),
        new SqlParameter("@Email", model.Email ?? ""),
        new SqlParameter("@Website", model.Website ?? ""),
        new SqlParameter("@GSTIN", model.GSTIN ?? ""),

        new SqlParameter("@BankName", model.BankName ?? ""),
        new SqlParameter("@AccountNo", model.AccountNo ?? ""),
        new SqlParameter("@IFSCCode", model.IFSCCode ?? ""),
        new SqlParameter("@LicenseNo", model.LicenseNo ?? ""),
        new SqlParameter("@PANNo", model.PANNo ?? ""),
        new SqlParameter("@UANNo", model.UANNo ?? ""),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
    };

            du.Execute("Sp_Insert_BranchInformation", prms);

            TempData["Msg"] = prms[^1].Value?.ToString();
            return RedirectToAction("BranchInfoList");
        }


        [HttpGet]
        public JsonResult GetBranchByCode(string ccode)
        {
            ViewBag.BranchTpyp = GetBranchTypeDDL();
            ViewBag.StateList = GetStatesDDL();
            ViewBag.BranchTpyp = GetBranchTypeDDL();
            ViewBag.CityList = new List<SelectListItem>();

            if (string.IsNullOrEmpty(ccode))
                return Json(null);

            SqlParameter[] prms =
            {
            new SqlParameter("@BCode", ccode)
    };

            DataTable dt = du.GetDataTable("Sp_Get_BranchByCode", prms);

            if (dt.Rows.Count == 0)
                return Json(null);

            var r = dt.Rows[0];

            var data = new
            {
                Id = r["Id"].ToString(),
                bcode = r["BCode"].ToString(),
                BranchName = r["BranchName"].ToString(),
                Abbr = r["Abbr"].ToString(),
                Address = r["Address"].ToString(),
                Address2 = r["Address2"].ToString(),
                CityId = r["CityId"].ToString(),
                StateId = r["StateId"].ToString(),
                BTypeId = r["BTypeId"].ToString(),
                Pincode = r["Pincode"].ToString(),
                Country = r["Country"].ToString(),
                Telepohne = r["Telepohne"].ToString(),
                ContactNo = r["ContactNo"].ToString(),
                Email = r["Email"].ToString(),
                Website = r["Website"].ToString(),
                GSTIN = r["GSTIN"].ToString(),
                BankName = r["BankName"].ToString(),
                AccountNo = r["AccountNo"].ToString(),
                IFSCCode = r["IFSCCode"].ToString(),
                LicenseNo = r["LicenseNo"].ToString(),
                PANNo = r["PANNo"].ToString(),
                UANNo = r["UANNo"].ToString()
            };

            return Json(data);
        }


        [HttpPost]
        public IActionResult DeleteBranch(string bcode)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@BCode", bcode)
        };

            du.Execute("Sp_Delete_MasterBranch", prms);
            TempData["Msg"] = "Record Deleted Successfully";

            return RedirectToAction("BranchInfoList");
        }


        private List<DateTime> GetAllDates(int month, int year)
        {
            var dates = new List<DateTime>();

            int daysInMonth = DateTime.DaysInMonth(year, month);
            for (int day = 1; day <= daysInMonth; day++)
            {
                dates.Add(new DateTime(year, month, day));
            }

            return dates;
        }


        private List<HolidayVM> GetHolidays(int month, int year)
        {
            SqlParameter[] prms =
            {
        new SqlParameter("@Month", month),
        new SqlParameter("@Year", year)
    };

            DataTable dt = du.GetDataTable("Sp_GetHolidaysByMonth", prms);

            List<HolidayVM> list = new List<HolidayVM>();

            foreach (DataRow row in dt.Rows)
            {
                list.Add(new HolidayVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    Holiday = row["Holiday"].ToString(),
                    Date = Convert.ToDateTime(row["Date"])
                });
            }

            return list;
        }
        private List<int> GetWeeklyOffDays()
        {
            DataTable dt = du.GetDataTableByQuery("SELECT OffDay FROM Tbl_WeeklyOff", null);

            List<int> list = new List<int>();
            foreach (DataRow r in dt.Rows)
                list.Add(Convert.ToInt32(r["OffDay"]));

            return list;
        }


        public IActionResult WorkingCalenderList(int month = 0, int year = 0)
        {
            if (month == 0) month = DateTime.Now.Month;
            if (year == 0) year = DateTime.Now.Year;

            var dates = GetAllDates(month, year);
            var holidays = GetHolidays(month, year);
            var saved = GetSavedCalendar(month, year);


            var weeklyOff = GetWeeklyOffDays();

            var model = dates.Select(d =>
            {
                var existing = saved.FirstOrDefault(x => x.WorkDate.Date == d.Date);

                if (existing != null) return existing;

                return new WorkingCalendarVM
                {
                    WorkDate = d,

                    Status = holidays.Any(h => h.Date.Date == d.Date)
            ? "Holiday"
            : weeklyOff.Contains((int)d.DayOfWeek)
                ? "WeeklyOff"
                : "Working"
                };
            }).ToList();

            ViewBag.Month = month;
            ViewBag.Year = year;

            return View(model);
        }

        [HttpPost]
        public IActionResult SaveWorkingCalender(WorkingCalendarVM model)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@WorkDate", model.WorkDate),
            new SqlParameter("@Status", model.Status),
            new SqlParameter("@HolidayId", model.HolidayId ?? (object)DBNull.Value),
            new SqlParameter("@Remarks", model.Remarks ?? "")
        };

            du.Execute("Sp_Save_WorkingCalendar", prms);

            TempData["Msg"] = "Saved successfully";
            return RedirectToAction("WorkingCalenderList");
        }

        [HttpPost]
        public IActionResult DeleteWorkingCalender(DateTime date)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@WorkDate", date)
        };

            du.Execute("Sp_Delete_WorkingCalendar", prms);

            TempData["Msg"] = "Reset to Working Day";
            return RedirectToAction("WorkingCalenderList");
        }

        private List<WorkingCalendarVM> GetSavedCalendar(int month, int year)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@Month", month),
            new SqlParameter("@Year", year)
        };

            DataTable dt = du.GetDataTable("Sp_Get_WorkingCalendar", prms);

            List<WorkingCalendarVM> list = new List<WorkingCalendarVM>();
            foreach (DataRow r in dt.Rows)
            {
                list.Add(new WorkingCalendarVM
                {
                    Id = Convert.ToInt32(r["Id"]),
                    WorkDate = Convert.ToDateTime(r["WorkDate"]),
                    Status = r["Status"].ToString(),
                    HolidayId = r["HolidayId"] == DBNull.Value ? null : Convert.ToInt32(r["HolidayId"]),
                    Remarks = r["Remarks"].ToString()
                });
            }
            return list;
        }
        public IActionResult LoadCalendar(int month, int year)
        {
            var dates = GetAllDates(month, year);
            var holidays = GetHolidays(month, year);

            var model = dates.Select(d => new WorkingCalendarVM
            {
                WorkDate = d,
                Status = holidays.Any(h => h.Date.Date == d.Date) ? "Holiday" : d.DayOfWeek == DayOfWeek.Sunday
                                ? "WeeklyOff" : "Working",
                HolidayName = holidays.FirstOrDefault(h => h.Date.Date == d.Date)?.Holiday
            }).ToList();
            return View(model);
        }

        [HttpPost]
        public IActionResult SaveWeeklyOff(List<int> OffDays)
        {
            du.ExecuteScalar("TRUNCATE TABLE Tbl_WeeklyOff");

            foreach (var d in OffDays)
            {
                du.ExecuteScalar("INSERT INTO Tbl_WeeklyOff(OffDay) VALUES (" + d + ")");
            }

            TempData["Msg"] = "Weekly off saved successfully";
            return RedirectToAction("WorkingCalenderList");
        }

        //EMPLOYEE TYPE
        public IActionResult AddEmployeeType(EmployeeTypeVM model)
        {
            SqlParameter[] prms =
        {
        new SqlParameter("@Id", model.Id),
        new SqlParameter("@EmployeeType", model.EmployeeType),

        new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
          };

            du.Execute("Sp_Insert_MasterEmployeeType", prms);
            string message = prms[2].Value.ToString();
            TempData["Msg"] = message;
            return RedirectToAction("EmployeeTypeList");
        }
        public IActionResult EmployeeTypeList()
        {
            DataTable dt = du.GetDataTable("Sp_Get_EmployeeTypeList", null);

            List<EmployeeTypeVM> list = new();
            foreach (DataRow row in dt.Rows)
            {
                list.Add(new EmployeeTypeVM
                {
                    Id = Convert.ToInt32(row["Id"]),
                    EmployeeType = row["EmployeeType"].ToString()
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult DeleteEmployeeType(int id)
        {
            SqlParameter[] prms =
            {
            new SqlParameter("@Id", id),
              new SqlParameter("@msg", SqlDbType.NVarChar, 100)
        {
            Direction = ParameterDirection.Output
        }
        };
            du.Execute("Sp_Delete_EmployeeType", prms);
            return RedirectToAction("EmployeeTypeList");
        }


        ///LATE OT RALEs

        public IActionResult AttendancePolicyList()
        {
            var dt = du.GetDataTableByQuery(
                "SELECT PolicyId,PolicyName,IsActive,CreatedOn FROM Tbl_AttendancePolicy",
                null);

            return View(dt);
        }


        public IActionResult Builder(int policyId)
        {
            ViewBag.Departments = du.GetDataTableByQuery(
                "SELECT Id,Department FROM Tbl_MasterDepartment", null);

            ViewBag.Employees = du.GetDataTableByQuery(
                "SELECT Emp_Id,Name FROM Tbl_MasterEmployeeDetails", null);

            PolicyFullVM vm = new PolicyFullVM();
            vm.PolicyId = policyId;

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult SaveFullPolicy(PolicyFullVM vm)
        {
                    SqlParameter[] pMaster =
           {
                new SqlParameter("@PolicyName","Company Policy"),
                new SqlParameter("@EffectiveFrom",DateTime.Today),
                new SqlParameter("@AppliesTo",vm.ApplyTo),
                new SqlParameter("@PolicyId",SqlDbType.Int)
                {
                    Direction = ParameterDirection.Output
                }
            };

            du.Execute("Sp_Insert_AttendancePolicy", pMaster);

            int policyId = Convert.ToInt32(pMaster[3].Value);

            // LATE
            SqlParameter[] p =
             {
                new SqlParameter("@PolicyId", vm.PolicyId),
                new SqlParameter("@GraceMinutes", vm.GraceMinutes),
                new SqlParameter("@GraceAction", vm.GraceAction)
                };
            du.Execute("Sp_Insert_PolicyLateRule", p);

            // PENALTY
            SqlParameter[] p1 =
              {

                new SqlParameter("@PolicyId", vm.PolicyId),
                new SqlParameter("@AllowedLate", vm.AllowedLate),
                new SqlParameter("@PenaltyType", vm.ApplyTo),
                new SqlParameter("@Deduction", vm.Deduction)
                };
            du.Execute("Sp_Insert_PolicyLatePenalty", p1);

            // DEPARTMENT MAP
            if (vm.ApplyTo == "department wise" && !string.IsNullOrEmpty(vm.DepartmentIds))
            {
                SqlParameter[] p4 =
                   {
                    new SqlParameter("@PolicyId", vm.PolicyId),
                    new SqlParameter("@DepartmentIds", vm.DepartmentIds)
                   };
                du.Execute("Sp_Insert_PolicyDepartmentMap", p4);
            }

            // OT
            SqlParameter[] p5 =
                  {
                new SqlParameter("@PolicyId", vm.PolicyId),
                new SqlParameter("@MinOT", vm.MinOTMinutes),
                new SqlParameter("@CalcType", vm.CalcType),
                new SqlParameter("@RoundTo", vm.RoundTo)
                };
            du.Execute("Sp_Insert_PolicyOTRule", p5);
            return Ok("Saved");
        }

        public IActionResult EditPolicy(int id)
        {
            var dt = du.GetScalar(
                "SELECT * FROM Tbl_AttendancePolicy WHERE PolicyId=@id", new SqlParameter("@id", id));

            //if (dt.Rows.Count == 0) return NotFound();

            PolicyFullVM vm = new PolicyFullVM
            {
                PolicyId = id,

                GraceMinutes = Convert.ToInt32(
                    du.GetScalar("SELECT GraceMinutes FROM Tbl_PolicyLateRule WHERE PolicyId=@id",
                        new SqlParameter("@id", id)) ?? 0),

                GraceAction = du.GetScalar("SELECT GraceAction FROM Tbl_PolicyLateRule WHERE PolicyId=@id",
                    new SqlParameter("@id", id))?.ToString(),

                AllowedLate = Convert.ToInt32(
                    du.GetScalar("SELECT AllowedLate FROM Tbl_PolicyLatePenalty WHERE PolicyId=@id",
                        new SqlParameter("@id", id)) ?? 0),

                Deduction = Convert.ToDecimal(
                    du.GetScalar("SELECT Deduction FROM Tbl_PolicyLatePenalty WHERE PolicyId=@id",
                        new SqlParameter("@id", id)) ?? 0),

                MinOTMinutes = Convert.ToInt32(
                    du.GetScalar("SELECT MinOTMinutes FROM Tbl_PolicyOTRule WHERE PolicyId=@id",
                        new SqlParameter("@id", id)) ?? 0),

                CalcType = du.GetScalar(
                    "SELECT CalcType FROM Tbl_PolicyOTRule WHERE PolicyId=@id",
                    new SqlParameter("@id", id))?.ToString()
            };

            ViewBag.Departments = du.GetDataTableByQuery(
                "SELECT Id,Department FROM Tbl_MasterDepartment", null);

            ViewBag.SelectedDepartments = du.GetScalar(
                "SELECT DepartmentId FROM Tbl_PolicyDepartmentMap WHERE PolicyId=@id",
                new SqlParameter("@id", id));

            return View("Builder", vm);
        }
    }
}

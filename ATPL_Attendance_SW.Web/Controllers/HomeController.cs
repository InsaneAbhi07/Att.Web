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
                    Date = row["Date"].ToString(),
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
                    ShiftId = row["ShiftId"] == DBNull.Value ? 0 : Convert.ToInt32(row["ShiftId"]),

                    EmailId = row["EmailId"] == DBNull.Value ? "" : row["EmailId"].ToString(),
                    PhoneNo = row["PhoneNo"] == DBNull.Value ? "" : row["PhoneNo"].ToString(),

                    JoiningDate = row["JoiningDate"] == DBNull.Value ? "" : row["JoiningDate"].ToString(),
                    Address = row["Address"] == DBNull.Value ? "" : row["Address"].ToString(),
                    DOB = row["Dob"] == DBNull.Value ? "" : row["Dob"].ToString(),

                    Emp_Img = row["Emp_Img"] == DBNull.Value ? "" : row["Emp_Img"].ToString(),
                    Designation = row["Designation"] == DBNull.Value ? "" : row["Designation"].ToString(),
                    Department = row["Department"] == DBNull.Value ? "" : row["Department"].ToString(),

                    Salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]),
                    Status = row["Status"] == DBNull.Value ? "Inactive" : row["Status"].ToString(),
                    Shift = row["Shift"] == DBNull.Value ? "" : row["Shift"].ToString(),

                    BankName = row["BankName"] == DBNull.Value ? "" : row["Shift"].ToString(),
                    Ac_HolderName = row["Ac_HolderName"] == DBNull.Value ? "" : row["Shift"].ToString(),
                    IFSC_Code = row["IFSC_Code"] == DBNull.Value ? "" : row["Shift"].ToString(),
                    Acc_No = row["Acc_No"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Acc_No"]),
                    EmpType = row["EmpType"] == DBNull.Value ? "" : row["EmpType"].ToString(),

                    UserName = row["UserName"] == DBNull.Value ? "" : row["UserName"].ToString(),
                    Role = row["Role"] == DBNull.Value ? "" : row["Role"].ToString()
                });


            }

            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
            ViewBag.Shiftlistt = GetSHiftDDL();

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
                    ShiftId = row["ShiftId"] == DBNull.Value ? 0 : Convert.ToInt32(row["ShiftId"]),

                    EmailId = row["EmailId"]?.ToString() ?? "",
                    PhoneNo = row["PhoneNo"]?.ToString() ?? "",

                    JoiningDate = row["JoiningDate"]?.ToString() ?? "",
                    Address = row["Address"]?.ToString() ?? "",
                    DOB = row["Dob"]?.ToString() ?? "",

                    Emp_Img = row["Emp_Img"]?.ToString() ?? "",
                    Designation = row["Designation"]?.ToString() ?? "",
                    Department = row["Department"]?.ToString() ?? "",

                    Salary = row["Salary"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Salary"]),
                    Status = row["Status"]?.ToString() ?? "Inactive",
                    Shift = row["Shift"]?.ToString() ?? "",

                    BankName = row["BankName"]?.ToString() ?? "",
                    Ac_HolderName = row["AC_HolderName"]?.ToString() ?? "",
                    IFSC_Code = row["IFSC_Code"]?.ToString() ?? "",
                    Acc_No = row["Acc_No"] == DBNull.Value ? 0 : Convert.ToDecimal(row["Acc_No"]),

                    EmpType = row["EmpType"]?.ToString() ?? "",
                    UserName = row["UserName"]?.ToString() ?? "",
                    Role = row["Role"]?.ToString() ?? ""
                });
            }

            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
            ViewBag.Shiftlistt = GetSHiftDDL();

            return View(list);
        }

        public IActionResult SaveEmployee(long id = 0)
        {
            ViewBag.DepartmentList = GetDepartmentDDL();
            ViewBag.DesignationList = GetDesignationDDL();
            ViewBag.Shiftlistt = GetSHiftDDL();
            ViewBag.WorkUnderList = GetWorkUnderDDL();

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
                EmpType = r["EmpType"]?.ToString(),


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
        new SqlParameter("@ShiftId", model.ShiftId),
        new SqlParameter("@JoiningDate", model.JoiningDate ?? (object)DBNull.Value),
        new SqlParameter("@ResignDate", model.ResignDate ?? (object)DBNull.Value),
        new SqlParameter("@PhoneNo", model.PhoneNo),
        new SqlParameter("@EmailId", model.EmailId ?? (object)DBNull.Value),
        new SqlParameter("@Address", model.Address ?? (object)DBNull.Value),
        new SqlParameter("@Gender", model.Gender ?? (object)DBNull.Value),
        new SqlParameter("@MaritalStatus", model.MaritalStatus ?? (object)DBNull.Value),
        new SqlParameter("@EmergencyContact", model.EmergencyContact ?? (object)DBNull.Value),
        new SqlParameter("@EmpType", model.EmpType ?? (object)DBNull.Value),
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
                    BranchType= r["BranchType"].ToString(), 
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
    }
    }

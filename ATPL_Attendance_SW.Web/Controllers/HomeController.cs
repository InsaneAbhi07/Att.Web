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

        [HttpPost]
        [HttpPost]
        [HttpPost]
        public IActionResult SaveEmployee(EmployeeVM model, IFormFile EmpImage)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(model.Name))
                {
                    TempData["Msg"] = "Employee Name is required";
                    return RedirectToAction("EmployeeList");
                }

                string imageName = model.Emp_Img ?? "";

                if (EmpImage != null && EmpImage.Length > 0)
                {
                    string uploadPath = Path.Combine(
                        Directory.GetCurrentDirectory(),
                        "wwwroot/uploads/employee"
                    );

                    if (!Directory.Exists(uploadPath))
                        Directory.CreateDirectory(uploadPath);

                    imageName = Guid.NewGuid() + Path.GetExtension(EmpImage.FileName);
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

            new SqlParameter("@Dob",
                string.IsNullOrEmpty(model.DOB)
                ? (object)DBNull.Value
                : model.DOB),

            new SqlParameter("@DepartmentId",
                model.DepartmentId == 0
                ? (object)DBNull.Value
                : model.DepartmentId),

            new SqlParameter("@DesignationId",
                model.DesignationId == 0
                ? (object)DBNull.Value
                : model.DesignationId),

            new SqlParameter("@ShiftId",
                model.ShiftId == 0
                ? (object)DBNull.Value
                : model.ShiftId),

            new SqlParameter("@EmailId", model.EmailId ?? ""),
            new SqlParameter("@PhoneNo", model.PhoneNo ?? ""),

            new SqlParameter("@JoiningDate",
                string.IsNullOrEmpty(model.JoiningDate)
                ? (object)DBNull.Value
                : model.JoiningDate),

            new SqlParameter("@Address", model.Address ?? ""),

            new SqlParameter("@Emp_Img", imageName),

            new SqlParameter("@Salary",
                model.Salary == 0
                ? (object)DBNull.Value
                : model.Salary),

            new SqlParameter("@Status", model.Status ?? "Active"),
            new SqlParameter("@UserName", model.UserName ?? ""),
            new SqlParameter("@Password", model.Password ?? ""),
            new SqlParameter("@Role", model.Role ?? ""),

            // BANK DETAILS
            new SqlParameter("@BankName", model.BankName ?? ""),
            new SqlParameter("@Ac_HolderName", model.Ac_HolderName ?? ""),
            new SqlParameter("@IFSC_Code", model.IFSC_Code ?? ""),

            // EMP TYPE (FIXED)
            new SqlParameter("@EmpType", model.EmpType ?? ""),

            new SqlParameter("@Acc_No", model.Acc_No == 0 ? (object)DBNull.Value : model.Acc_No),

            new SqlParameter("@msg", SqlDbType.NVarChar, 100)
            {
                Direction = ParameterDirection.Output
            }
        };
                du.Execute("Sp_Insert_MasterEmployee", prms);

                TempData["Msg"] = prms[^1].Value?.ToString() ?? "Saved successfully";
            }
            catch (Exception ex)
            {
                TempData["Msg"] = "Something went wrong. Please try again.";
            }
            return RedirectToAction("EmployeeList");
        }

        [HttpGet]
        [HttpGet]
        public IActionResult CheckUsername(string username, string oldUsername)
        {
             if (username == oldUsername)
                return Json(false);

            SqlParameter[] prms =
            {
              new SqlParameter("@UserName", username)
            };

            int count = Convert.ToInt32(
                du.Execute("Sp_CheckUsernameExissts", prms)
            );

            return Json(count > 0);
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

            DataTable dt = du.GetDataTableByQuery("SELECT Id, City FROM Tbl_MasterCity WHERE StateId = @StateId",param);
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
                    IFSCCode = r["IFSCCode"].ToString()


                });
            }

            return View(list);
        }


        [HttpPost]
        [HttpPost]

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

        new SqlParameter("@Telepohne", model.Telepohne ?? ""),
        new SqlParameter("@ContactNo", model.ContactNo ?? ""),
        new SqlParameter("@Email", model.Email ?? ""),
        new SqlParameter("@Website", model.Website ?? ""),
        new SqlParameter("@GSTIN", model.GSTIN ?? ""),

        new SqlParameter("@BankName", model.BankName ?? ""),
        new SqlParameter("@AccountNo", model.AccountNo ?? ""),
        new SqlParameter("@IFSCCode", model.IFSCCode ?? ""),

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
                StateId= r["StateId"].ToString(),
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
                    BreakIn = row["BreakIn"].ToString()
                });
            }

            return View(list);
        }

        [HttpPost]
        public IActionResult AddShift(MasterShift model)
        {
            SqlParameter[] prms = { new SqlParameter("@Id", model.Id),

                new SqlParameter("@Shift",string.IsNullOrWhiteSpace(model.Shift)? (object)DBNull.Value: model.Shift),

                new SqlParameter("@ShiftType",string.IsNullOrWhiteSpace(model.ShiftType)? (object)DBNull.Value:model.ShiftType),

                new SqlParameter("@InTime", string.IsNullOrWhiteSpace(model.InTime)? (object)DBNull.Value: model.InTime),

                new SqlParameter("@OutTime",string.IsNullOrWhiteSpace(model.OutTime)? (object)DBNull.Value: model.OutTime),

                new SqlParameter("@BreakOut",string.IsNullOrWhiteSpace(model.BreakOut)? (object)DBNull.Value: model.BreakOut),

                new SqlParameter("@BreakIn",string.IsNullOrWhiteSpace(model.BreakIn)? (object)DBNull.Value: model.BreakIn),

                new SqlParameter("@msg", SqlDbType.NVarChar, 100)
                {
                    Direction = ParameterDirection.Output
                }
            };
            du.Execute("Sp_Save_MasterShift", prms);
            TempData["Msg"] = prms[7].Value.ToString();
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
                    Total_Y = Convert.ToInt32(row["Total_Y"])
                });
            }
            return View(list);
        }

        // ADD / UPDATE
        [HttpPost]
        public IActionResult AddLeaveType(LeaveTypeVM model)
        {
            SqlParameter[] prms = { new SqlParameter("@Id", model.Id),

                new SqlParameter("@LeaveType",string.IsNullOrWhiteSpace(model.LeaveType)? (object)DBNull.Value: model.LeaveType),

                new SqlParameter("@Penalty", (object?)model.Penalty ?? DBNull.Value),
                new SqlParameter("@Total_Y", (object?)model.Total_Y ?? DBNull.Value),

                new SqlParameter("@msg", SqlDbType.NVarChar, 100)
                {
                    Direction = ParameterDirection.Output
                }
            };
            du.Execute("Sp_Insert_Master_LeaveType", prms);
            TempData["Msg"] = prms[4].Value.ToString();
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
            SqlParameter[] prms = { new SqlParameter("@Id", model.Id),

                new SqlParameter("@StateId", (object?)model.StateId ?? DBNull.Value),
                new SqlParameter("@City",string.IsNullOrWhiteSpace(model.City)? (object)DBNull.Value: model.City),
                new SqlParameter("@msg", SqlDbType.NVarChar, 100)
                {
                    Direction = ParameterDirection.Output
                }
            };
            du.Execute("Sp_InsertMasterCity", prms);
            TempData["Msg"] = prms[3].Value.ToString();
            return RedirectToAction("MasterStateCityList");

        }


    }
}

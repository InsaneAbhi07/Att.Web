using System.Data;
using ATPL_Attendance_SW.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;

namespace ATPL_Attendance_SW.Web.Controllers
{
    [Authorize(AuthenticationSchemes = "AttendanceCookie")]

    public class AttendanceController : Controller
    {
        private readonly ILogger<AttendanceController> _logger;
        private readonly DataUtility du;

        public AttendanceController(
            ILogger<AttendanceController> logger,
            IConfiguration config)
        {
            _logger = logger;
            du = new DataUtility(config);
        }

        public IActionResult LateRuleList()
        {
            var dt = du.GetDataTable("Sp_GetLateRules", null);
            return View(dt);
        }

        [HttpPost]
        public IActionResult SaveLateRule(LateRuleVM vm)
        {
            SqlParameter[] prms = new SqlParameter[]
            {
            new SqlParameter("@LateRuleId", vm.LateRuleId),
            new SqlParameter("@RuleName", vm.RuleName),
            new SqlParameter("@LateAfterMinutes", vm.LateAfterMinutes),
            new SqlParameter("@PenaltyAfterMinutes", vm.PenaltyAfterMinutes),
            new SqlParameter("@PenaltyType", vm.PenaltyType),
            new SqlParameter("@PenaltyValue", vm.PenaltyValue),
            new SqlParameter("@MaxAllowedInMonth", vm.MaxAllowedInMonth),
            };

            du.Execute("Sp_InsertLateRule", prms);
            TempData["Msg"] = "Late rule saved successfully";
            return RedirectToAction("LateOTView");
        }

        public IActionResult OTRuleList()
        {
            DataTable dt = du.GetDataTable("Proc_OTRule_GetAll", null);
            return View(dt);
        }

        [HttpPost]
        public IActionResult SaveOTRule(OTRuleVM vm)
        {
            SqlParameter[] p = {
        new("@OTRuleId", vm.OTRuleId),
        new("@RuleName", vm.RuleName),
        new("@OTAfterMinutes", vm.OTAfterMinutes),
        new("@MinOTMinutes", vm.MinOTMinutes),
        new("@MaxOTPerDay", vm.MaxOTPerDay),
        new SqlParameter("@IsHolidayAllowed", vm.IsHolidayAllowed ? 1 : 0),
        new SqlParameter("@IsWeekOffAllowed", vm.IsWeekOffAllowed ? 1 : 0),

    };

            du.Execute("Sp_Insert_OTRule", p);
            return RedirectToAction("LateOTView");
        }

        public IActionResult RuleApply()
        {
            ViewBag.Departments = du.GetDataTableByQuery(
                "SELECT Id,Department FROM Tbl_MasterDepartment", null);

            ViewBag.Shifts = du.GetDataTableByQuery(
                "SELECT Id,Shift FROM Tbl_MasterShift", null);

            ViewBag.LateRules = du.GetDataTable("Sp_GetLateRules", null);
            ViewBag.OTRules = du.GetDataTable("Proc_OTRule_GetAll", null);

            return View();
        }


        public IActionResult LateOTView()
        {
            ViewBag.LateRules = du.GetDataTable("Sp_GetLateRules", null);
            ViewBag.OTRules = du.GetDataTable("Proc_OTRule_GetAll", null);
            ViewBag.RuleApply = du.GetDataTableByQuery("SELECT * FROM Tbl_AttendanceRuleApply", null);

            ViewBag.Departments = du.GetDataTableByQuery(
                "SELECT Id,Department FROM Tbl_MasterDepartment", null);

            ViewBag.Shifts = du.GetDataTableByQuery(
                "SELECT Id,Shift FROM Tbl_MasterShift", null);

            ViewBag.RuleApplyList = du.GetDataTableByQuery(@"
            SELECT A.ApplyId, A.RuleType, A.RuleId, D.Department, S.Shift 
            FROM Tbl_AttendanceRuleApply A
            LEFT JOIN Tbl_MasterDepartment D ON A.ApplyOnId = D.Id
            LEFT JOIN Tbl_MasterShift S ON A.ShiftId = S.Id
            ORDER BY A.ApplyId DESC", null);

            return View();
        }

        [HttpPost]
        [HttpPost]
        public IActionResult SaveRuleApply(RuleApplyVM vm, int[] DepartmentIds)
        {
            if (vm.ApplyOnType == "2") 
            {
                foreach (var deptId in DepartmentIds)
                {
                    SaveSingleRule(vm, deptId);
                }
            }
            else
            {
                SaveSingleRule(vm, null);
            }

            TempData["Msg"] = "Rule applied successfully";
            return RedirectToAction("LateOTView");
        }

        void SaveSingleRule(RuleApplyVM vm, int? deptId)
        {
            SqlParameter[] p =
            {
        new("@RuleType", vm.RuleType),
        new("@RuleId", vm.RuleId),
        new("@ApplyOnType", vm.ApplyOnType),
        new("@ApplyOnId", deptId ?? (object)DBNull.Value),
        new("@ShiftId", vm.ShiftId ?? (object)DBNull.Value)
    };
            du.Execute("SP_InsertRuleApply_Save", p);
        }

        public IActionResult DeleteApply(int id)
        {
            du.ExecuteNonQuery("UPDATE Tbl_AttendanceRuleApply SET IsActive=0 WHERE ApplyId=@id",
                new[] { new SqlParameter("@id", id) });

            TempData["Msg"] = "Deleted successfully";
            return RedirectToAction("LateOTView");
        }

        public IActionResult DeleteLate(int id)
        {
            du.ExecuteNonQuery("UPDATE Tbl_LateRuleMaster SET IsActive=0 WHERE LateRuleId=@id",
                new[] { new SqlParameter("@id", id) });
            TempData["Msg"] = "Deleted";
            return RedirectToAction("LateOTView");
        }

        public IActionResult DeleteOT(int id)
        {
            du.ExecuteNonQuery("UPDATE Tbl_OTRuleMaster1 SET IsActive=0 WHERE OTRuleId=@id",
                new[] { new SqlParameter("@id", id) });
            TempData["Msg"] = "Deleted";
            return RedirectToAction("LateOTView");
        }
    }
}

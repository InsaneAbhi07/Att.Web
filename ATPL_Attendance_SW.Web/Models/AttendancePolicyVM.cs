namespace ATPL_Attendance_SW.Web.Models
{
    public class AttendancePolicyVM
    {
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public bool IsActive { get; set; }

        // Late
        public int GraceMinutes { get; set; }
        public string GraceAction { get; set; }
        public int AllowedLate { get; set; }
        public decimal Deduction { get; set; }

        // OT
        public int MinOTMinutes { get; set; }
        public string OTCalcType { get; set; }
        public decimal OTRoundTo { get; set; }

        // Mapping
        public string ApplyTo { get; set; }
        public string DepartmentIds { get; set; }
    }
}

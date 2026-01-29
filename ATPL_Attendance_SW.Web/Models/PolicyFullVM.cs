namespace ATPL_Attendance_SW.Web.Models
{
    public class PolicyFullVM
    {
        public int PolicyId { get; set; }

        public DateTime PolicyDate { get; set; } = DateTime.Today;

        // Late
        public int GraceMinutes { get; set; }
        public string GraceAction { get; set; }
        public int AllowedLate { get; set; }
        public decimal Deduction { get; set; }
        public string ApplyTo { get; set; }
        public string DepartmentIds { get; set; }

        // OT
        public int MinOTMinutes { get; set; }
        public string CalcType { get; set; }
        public decimal RoundTo { get; set; }
    }
}

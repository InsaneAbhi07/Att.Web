namespace ATPL_Attendance_SW.Web.Models
{
    public class LateRuleVM
    {
        public int PolicyId { get; set; }
        public int GraceMinutes { get; set; }
        public string GraceAction { get; set; }
    }
}

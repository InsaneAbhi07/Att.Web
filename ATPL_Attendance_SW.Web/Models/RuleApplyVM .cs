namespace ATPL_Attendance_SW.Web.Models
{
    public class RuleApplyVM
    {
        public string RuleType { get; set; } 
        public int RuleId { get; set; }
        public string ApplyOnType { get; set; } 
        public int? ApplyOnId { get; set; } = null;

        public int? ShiftId { get; set; } = null;
    }
}

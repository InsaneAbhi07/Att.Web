namespace ATPL_Attendance_SW.Web.Models
{
    public class LateRuleVM
    {
        public int LateRuleId { get; set; }
        public string RuleName { get; set; }
        public int LateAfterMinutes { get; set; }
        public int PenaltyAfterMinutes { get; set; }
        public int PenaltyType { get; set; }
        public decimal PenaltyValue { get; set; }
        public int MaxAllowedInMonth { get; set; }
    }
}

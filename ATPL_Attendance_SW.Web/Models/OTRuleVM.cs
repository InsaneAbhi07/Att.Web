namespace ATPL_Attendance_SW.Web.Models
{
    public class OTRuleVM
    {
        public int PolicyId { get; set; }
        public int MinOTMinutes { get; set; }
        public string CalcType { get; set; }
        public decimal RoundTo { get; set; }
        public string ApprovalType { get; set; }
    }
}

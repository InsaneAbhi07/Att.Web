namespace ATPL_Attendance_SW.Web.Models
{
    public class OTRuleVM
    {
        public int OTRuleId { get; set; }
        public string RuleName { get; set; }
        public int OTAfterMinutes { get; set; }
        public int MinOTMinutes { get; set; }
        public int MaxOTPerDay { get; set; }
        public bool IsHolidayAllowed { get; set; }
        public bool IsWeekOffAllowed { get; set; }
    }
}

namespace ATPL_Attendance_SW.Web.Models
{
    public class MasterShift
    {
        public decimal Id { get; set; }
        public string? Shift { get; set; }
        public string? ShiftType { get; set; }
        public string? InTime { get; set; }
        public string? OutTime { get; set; }
        public string? BreakOut { get; set; }
        public string? BreakIn { get; set; }
        public string? CarryForward { get; set; }
        public string? Compensationin { get; set; }
    }

}

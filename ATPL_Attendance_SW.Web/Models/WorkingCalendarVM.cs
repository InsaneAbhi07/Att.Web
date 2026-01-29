namespace ATPL_Attendance_SW.Web.Models
{
    public class WorkingCalendarVM
    {
        public int Id { get; set; } 
        public DateTime WorkDate { get; set; }
        public string DayName => WorkDate.DayOfWeek.ToString();

        public string? Status { get; set; }   // Working / Holiday / WeeklyOff
        public int? HolidayId { get; set; }
        public string? HolidayName { get; set; }
        public string? Remarks { get; set; }
    }
}

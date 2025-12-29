namespace ATPL_Attendance_SW.Web.Models
{
    public class HolidayVM
    {
        public int Id { get; set; }
        public string Holiday { get; set; }

        public string Date { get; set; }
        public string Description { get; set; }
        public bool Active { get; set; }
    }
}

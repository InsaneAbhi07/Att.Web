namespace ATPL_Attendance_SW.Web.Models
{
    public class LateSlabVM
    {
        public int PolicyId { get; set; }
        public int FromMin { get; set; }
        public int ToMin { get; set; }
        public string Action { get; set; }
    }
}

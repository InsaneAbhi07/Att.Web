namespace ATPL_Attendance_SW.Web.Models
{
    public class PolicyTargetVM
    {
        public int PolicyId { get; set; }
        public string TargetType { get; set; }
        public List<int> TargetIds { get; set; }
    }
}

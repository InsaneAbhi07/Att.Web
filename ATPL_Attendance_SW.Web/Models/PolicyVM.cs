namespace ATPL_Attendance_SW.Web.Models
{
    public class PolicyVM
    {
        public int PolicyId { get; set; }
        public string PolicyName { get; set; }
        public DateTime EffectiveFrom { get; set; }
        public string AppliesTo { get; set; }
    }
}

namespace ATPL_Attendance_SW.Web.Models
{
    public class LatePenaltyVM
    {
        public int PolicyId { get; set; }
        public int AllowedLate { get; set; }
        public string PenaltyType { get; set; }
        public decimal Deduction { get; set; }
    }
}

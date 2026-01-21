using System.ComponentModel.DataAnnotations;

namespace ATPL_Attendance_SW.Web.Models
{
    public class LeaveTypeVM
    {
        public int Id { get; set; }
        [Required(ErrorMessage ="Please Enter Leave Type properly")]
        public string LeaveType { get; set; }
        [Required(ErrorMessage = "Please Enter No. of Days as Penalty")]
        public int Penalty { get; set; }
        public int Total_Y { get; set; }
        public string CarryForward { get; set; }
        public string? Compensationin { get; set; }
    }
}

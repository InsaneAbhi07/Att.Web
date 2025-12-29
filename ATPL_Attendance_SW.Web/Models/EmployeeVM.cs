using System.ComponentModel.DataAnnotations;

namespace ATPL_Attendance_SW.Web.Models
{
    public class EmployeeVM
    {
       // public int Id { get; set; }
        public int Emp_Id { get; set; }
        [Required(ErrorMessage ="This field is required.")]
        public string Name { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public string EmailId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string PhoneNo { get; set; }
        public string JoiningDate { get; set; }
        public string ?Address { get; set; }
        public string ?Emp_Img { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }
        public string Shift { get; set; }

        public string Department { get; set; }

        public string Designation { get; set; }
    }

}

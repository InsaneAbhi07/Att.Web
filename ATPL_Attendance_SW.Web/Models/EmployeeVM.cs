using System.ComponentModel.DataAnnotations;

namespace ATPL_Attendance_SW.Web.Models
{
    public class EmployeeVM
    {
        // public int Id { get; set; }
        public int Emp_Id { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string Emp_Code { get; set; }
        public string Name { get; set; }

        public string? DOB { get; set; }
        public int DepartmentId { get; set; }
        public int DesignationId { get; set; }
        public string EmailId { get; set; }
        [Required(ErrorMessage = "This field is required.")]
        public string PhoneNo { get; set; }
        public string? JoiningDate { get; set; }
        public string? Address { get; set; }
        public string? Emp_Img { get; set; }
        public decimal Salary { get; set; }
        public string Status { get; set; }
        public int ShiftId { get; set; }

        public string? UserName { get; set; }
        public string? Password { get; set; }
        public string? Role { get; set; }

        public string ResignDate { get; set; }
        public string Department { get; set; }
        public string Shift { get; set; }
        public string Designation { get; set; }
        public string EmployeeType { get; set; }

        public int WorkUnderId { get; set; }

        public string? BankName { get; set; }
        public string? Ac_HolderName { get; set; }
        public string? IFSC_Code { get; set;}
        public int EmpTypeId { get; set; }
        public decimal? Acc_No { get; set; }

        public string Gender { get; set; }
        public string MaritalStatus { get; set; }
        public string EmergencyContact { get; set; }

        public decimal BasicSalary { get; set; }
        public decimal HRA { get; set; }
        public decimal DA { get; set; }
        public decimal PA { get; set; }
        public decimal OtherAllowance { get; set; }
        public decimal GrossSalary { get; set; }
    
        public string ResumeDoc { get; set; }

        public string Aadhardoc { get; set; }

    }

}

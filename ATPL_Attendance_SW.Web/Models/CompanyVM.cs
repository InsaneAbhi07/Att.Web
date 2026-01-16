using System.ComponentModel.DataAnnotations;

namespace ATPL_Attendance_SW.Web.Models
{
    public class CompanyVM
    {
        public long Id { get; set; }
        public string CCode { get; set; }
        public string CompanyName { get; set; }
        public string? Abbr { get; set; }
        [Required(ErrorMessage ="This field is required")]
        public string? Address { get; set; }
        public string? Address2 { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public int CityId { get; set; }
        public int StateId { get; set; }
        public string? Pincode { get; set; }
        public string Country { get; set; }
        public string? Telepohne { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string? ContactNo { get; set; }
        public string? Email { get; set; }
        [Required(ErrorMessage = "This field is required")]

        public string? Website { get; set; }
        [Required(ErrorMessage = "This field is required")]
        public string? GSTIN { get; set; }
        public string? BankName { get; set; }
        public string? AccountNo { get; set; }
        public string? IFSCCode { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }


    }
}

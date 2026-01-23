using System.ComponentModel.DataAnnotations;

namespace ATPL_Attendance_SW.Web.Models
{
    public class MasterBranchVM
    {
            public long Id { get; set; }
            [Required(ErrorMessage = "Branch Code required")]
            public string? BCode { get; set; }
            [Required(ErrorMessage = "Branch Name required")]
            public string BranchName { get; set; }
            public string? Abbr { get; set; }
            [Required(ErrorMessage = "This field is required")]
            public int? BTypeId { get; set; }
            public string? Address { get; set; }
            public string? Address2 { get; set; }
            public int? CityId { get; set; }
            public int? StateId { get; set; }
            public string? Pincode { get; set; }
            public string? Country { get; set; }
            public string? Telepohne { get; set; }
            [Required(ErrorMessage = "This field is required")]
            public string? ContactNo { get; set; }
            public string? Email { get; set; }
            public string? Website { get; set; }
            //[Required(ErrorMessage = "This field is required")]
            public string? GSTIN { get; set; }
            public string? BankName { get; set; }
            public string? AccountNo { get; set; }
            public string? IFSCCode { get; set; }
            public string? City { get; set; }
            public string? State { get; set; }
            public string? UANNo { get; set; }
            public string? PANNo { get; set; }
            public string? LicenseNo { get; set; }
            public string? BranchType { get; set; }
    }

}

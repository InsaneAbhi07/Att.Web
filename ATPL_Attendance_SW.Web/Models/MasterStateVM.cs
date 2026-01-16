using System.ComponentModel.DataAnnotations;

namespace ATPL_Attendance_SW.Web.Models
{
    public class MasterStateVM
    {
        public int Id { get; set; }
        [Required (ErrorMessage = "Please enter State name properly")]
        public string State { get; set; }  
    }
}

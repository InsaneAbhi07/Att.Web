using System.ComponentModel.DataAnnotations;

namespace ATPL_Attendance_SW.Web.Models
{
    public class MasterCityVM
    {
        public int Id { get; set; } 
        public int StateId { get; set; }
        [Required(ErrorMessage = "Please enter City name properly")]

        public string City { get; set; }
    }
}

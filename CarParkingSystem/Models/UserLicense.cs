#nullable disable
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class UserLicense
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Driver First Name")]
        public string DfirstName { get; set; }

        [Required]
        [Display(Name = "Driver Last Name")]
        public string DlastName { get; set; }

        [Required]
        [Display(Name = "Driver license valid or not")]
        public bool isValid { get; set; }

        public long? idNumber { get; set; }

        [Required]
        [Display(Name = "User license Number ")]
        public string licenseNumber { get; set; } 
    }

}

#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.DTO
{
    public class ActiveDTO
    {
        [Required]
        public long OrgNumber { get; set; }
        [Required]
        public long IdNumber {  get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
    }
}

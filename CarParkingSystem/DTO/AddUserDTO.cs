#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarParkingSystem.DTO
{
    public class AddUserDTO
    {
        [Required]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Required]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public bool IsAccountActive { get; set; }
        [Required]
        [Display(Name ="13 digit Id number")]
        public long idNumber { get; set; }
        [Required]
        [Display(Name = "8 digit org number")]
        public long orgNum { get; set; }
        public string Email {  get; set; }
        [Required]
        [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }
        [DataType(DataType.Password)]
        [Display(Name = "Confirm password")]
        [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; }
        [Required]
        [Display(Name ="Phone")]
        public string Phone {  get; set; }  

        public string UserRole {  get; set; }   



    }
}

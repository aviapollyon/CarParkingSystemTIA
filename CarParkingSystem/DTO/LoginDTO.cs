#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.DTO
{
    public class LoginDTO
    {
        [Required]
        [Display(Name = "Org Number")]
        public long OrgNumer { get; set; }
        [Required]
        [Display(Name ="Password")]
        public string Password { get; set; }    
    }
}

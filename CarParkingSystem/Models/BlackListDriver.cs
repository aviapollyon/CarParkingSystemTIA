#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class BlackListDriver
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        public string RegPlate { get; set; }
        [Required]
        public string FirstName {  get; set; }
        [Required]
        public string LastName { get; set; }
        [Required]
        public string Purpose {  get; set; }    
    }
}

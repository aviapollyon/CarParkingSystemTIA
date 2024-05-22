#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class Vehicle
    {
        [Key]
        public int Id { get; set; }
        [Required]
        [Display(Name = "Vehicle Plate Number")]
        public string regPlateNum { get; set; }
        [Required]
        [Display(Name = "Vehicle Model")]
        public string vehicleModel { get; set; }

        [Required]
        [Display(Name = "Vehicle Make")]
        public string vehicleMake { get; set; }
        [Required]
        [Display(Name = "Vehicle Color")]
        public string vehicleColor { get; set; }
        [Required]
        [Display(Name = "Vehicle Registration Date")]
        public DateTime vehicleRegistrationDate { get; set; }   
        public long idNumber { get; set; }  
    }
}

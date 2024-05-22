#nullable disable

using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class Visitation
    {
        [Key]
        public int Id { get; set; }
        [Display(Name ="First Name")]     
        public string FirstNmae { get; set; }
        [Display(Name = "Last Name")]      
        public string LastNmae { get; set; }
        [Display(Name = "Purpose of visit")]
        [Required]
        public string purpose { get; set; }
        [Display(Name = "Registration Plate")]
        [Required]
        public string RegistrationPlate { get; set; }
        public string ExitTime { get; set; } 
        public DateTime EntryDate {  get; set; }  
        public string EntryTime {  get; set; }  
        public string ParkingBay {  get; set; } 
    }
}

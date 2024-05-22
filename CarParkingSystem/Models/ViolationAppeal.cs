#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class ViolationAppeal
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        public string ReasonAppeal {  get; set; }
        [Required]
        public string AppealMessage {  get; set; }
        public string Status {  get; set; } 
        public int ViolationId { get; set; }    
    }
}

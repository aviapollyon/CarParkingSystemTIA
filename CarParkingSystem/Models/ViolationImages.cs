#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class ViolationImages
    {
        [Key]
        public int Id { get; set; } 
        public string ViolationImage { get; set; }      
        public int StudentViolationId {  get; set; }    
    }
}

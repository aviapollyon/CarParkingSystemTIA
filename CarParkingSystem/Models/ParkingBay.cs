#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class ParkingBay
    {
        [Key]
        public int Id { get; set; } 
        public string BayId { get; set; }   
        public string BayNumber { get; set; }   
        public string BaySection { get; set; }  
        public string Occupacy { get; set; }    
    }
}

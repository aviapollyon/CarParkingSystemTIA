#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class EventCheckIn
    {
        [Key]
        public int Id { get; set; } 
        public string RegPlate { get; set; }    
        public string CheckInTime {  get; set; }
        public string CheckOutTime {  get; set; }   
        public int EventId {  get; set; }   
    }
}

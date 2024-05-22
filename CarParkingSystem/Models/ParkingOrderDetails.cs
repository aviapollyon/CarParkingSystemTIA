using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class ParkingOrderDetails
    {
        [Key]
        public int Id { get; set; } 
        public int PaymentId {  get; set; } 
        public int RevervationID { get; set; }  
        public long IdNumber {  get; set; } 
    }
}

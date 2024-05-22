#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class EventPayment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string DateTime { get; set; }
        [Required]
        public string CardCVV { get; set; }
        public int EventId {  get; set; }   
    }
}

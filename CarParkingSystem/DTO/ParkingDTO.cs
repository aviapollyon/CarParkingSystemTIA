#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.DTO
{
    public class ParkingDTO
    {
        public string BayID { get; set; }   
        public string RegNumber { get; set; }   
        public DateTime ReservationDateFrom { get; set; }   
        public DateTime ReservationDateTo {  get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string DateTime { get; set; }
        [Required]
        public string CardCVV { get; set; }

        public decimal TotalAmount {  get; set; }   

      
    }
}

#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarParkingSystem.Models
{
    public class ViolationPayment
    {
        [Key]
        public int Id { get; set; }
        [Required]
        public string CardNumber { get; set; }
        [Required]
        public string DateTime { get; set; }
        [Required]
        public string CardCVV { get; set; }

        public decimal TotalAmount { get; set; }
        public int ViolationId { get; set; }
    }
}

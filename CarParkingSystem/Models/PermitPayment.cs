#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarParkingSystem.Models
{
    public class PermitPayment
    {
        [Key]
        public int Id { get; set; }
        [Required] 
        public string CardNumber { get; set; }
        [Required]
        public string DateTime { get; set; }
        [Required]
        public string CardCVV { get; set; }
        public long IdNumber { get; set; }
        public string PaymentMode { get; set; }
        [NotMapped]
        public string[] selectedIds {  get; set; }  

        public decimal TotalAmount { get; set; }     
        public string RegNumber {  get; set; }  
    }
}

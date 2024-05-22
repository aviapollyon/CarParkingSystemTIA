#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class OrderDetails
    {
        [Key]
        public int Id { get; set; }
        public string IdNumber {  get; set; }           
        public int PermitID { get; set; }   
        public int PermitPaymentID { get; set; }    
    }
}

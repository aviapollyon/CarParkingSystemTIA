#nullable disable
namespace CarParkingSystem.ViewModel
{
    public class InvoiceViewModel
    {
        public long QrCode {  get; set; }   
        public DateTime OrderConfirmDate { get; set; }    
        public string FirstName { get; set; }   
        public string LastName { get; set; }    
        public string Email { get; set; }   
        public string Phone { get; set; }
        public string? RegNumber {  get; set; }  
        public string? BayId {  get; set; }  
        public decimal? PermitAmount { get; set; }
        public decimal? ParkingAmount {  get; set; } 
        public decimal? TotalAmunt {  get; set; }

        public string day {  get; set; }    
    }
}

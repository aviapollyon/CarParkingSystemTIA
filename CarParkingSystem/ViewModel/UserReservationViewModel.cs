#nullable disable
namespace CarParkingSystem.ViewModel
{
    public class UserReservationViewModel
    {
        public string BayId { get; set; }   
        public string Section { get; set;  }
        public string Bay {  get; set; }    
        public DateTime ExpiryDate {  get; set; }   
        public string OrgNumr { get; set; }  
    }
}

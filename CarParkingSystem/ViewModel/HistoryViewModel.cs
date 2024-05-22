#nullable disable
namespace CarParkingSystem.ViewModel
{
    public class HistoryViewModel
    {
        public long OrgNumber {  get; set; }    
        public string FullName {  get; set; }   
        public string BayId {  get; set; }  
        public DateTime CheckIn {  get; set; }  
        public DateTime? CheckOut { get; set; }  
    }
}

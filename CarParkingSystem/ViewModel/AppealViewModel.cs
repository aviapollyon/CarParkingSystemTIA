#nullable disable
using CarParkingSystem.Models;

namespace CarParkingSystem.ViewModel
{
    public class AppealViewModel
    {
        public StudentAppealViewModel StudentAppeal { get; set; }   
        public List<ViolationImages> ViolationImages { get; set; }  
    }
}

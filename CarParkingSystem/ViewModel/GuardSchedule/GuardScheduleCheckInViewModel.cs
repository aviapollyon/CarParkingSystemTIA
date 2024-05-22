#nullable disable
using CarParkingSystem.Models;

namespace CarParkingSystem.ViewModel.GuardSchedule
{
    public class GuardScheduleCheckInViewModel
    {
        public List<Models.GuardSchedule> GuardSchedulesList { get; set; } 
        public long LoginUserOrgNumber { get; set; }  
    }
}

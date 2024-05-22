#nullable disable
namespace CarParkingSystem.ViewModel.GuardSchedule
{
    public class GuardScheduleViewModel
    {
        public long OrgNumber { get; set; } 
        public string FirstName { get; set; }   
        public string LastName { get; set; }
        public string OnDutyTime { get; set; }
        public string OffDutyTime { get; set; }
        public string ToBreakTime { get; set; }
        public string FromBreakTime { get; set; }
    }
}

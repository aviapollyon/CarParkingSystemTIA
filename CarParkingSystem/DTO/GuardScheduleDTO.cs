#nullable disable
namespace CarParkingSystem.DTO
{
    public class GuardScheduleDTO
    {
        public string OnDutyTime { get; set; }  
        public string OffDutyTime { get; set; } 
        public string ToBreakTime {  get; set; }    
        public string FromBreakTime { get; set;}
    }
}

#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class GuardSchedule
    {
        [Key]
        public int Id { get; set; }
        public long OrgNumber { get; set; }
        public string FirstName {  get; set; }  
        public string LastName { get; set; }
        public string OnDutyTime { get; set; }
        public string OffDutyTime { get; set; }   
        public string ToBreakTime {  get; set; }    
        public string FromBreakTime {  get; set; }    
        public string Status { get; set; }  
        public string LeaveTimeTo {  get; set; }    
        public string LeaveTimeFrom {  get; set; }
    }
}

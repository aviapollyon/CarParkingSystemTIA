#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class CheckIn
    {
        [Key]
        public int Id { get; set; }
        public long OrgNum {  get; set; }
        public string regPlateNum { get; set; }
        public string  bayID { get; set; }    
        public DateTime CheckInTime {  get; set; }
        public string CheckInBay {  get; set; } 
    }
}

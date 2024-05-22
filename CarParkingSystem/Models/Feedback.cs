#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class Feedback
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        public int RatingNumber { get; set; }    
        public DateTime RatingDate { get; set; }  
        public string OrgNumber {  get; set; }
        [Required]
        public string UserMessage { get; set; } 
        public string AdminReplay {  get; set; }    
        public string FeedbackStatus {  get; set; } 
    }
}

#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarParkingSystem.Models
{
    public class StudentViolation
    {
        [Key]
        public int Id { get; set; } 
        public long OrgNumber { get; set; }
        public string RegPlate { get; set; }    
        public string Severity {  get; set; }
        [Required]
        public string PartiesInvolved { get; set; }
        [Required]
        public string AdditionInformation {  get; set; }       
        public decimal ServerityFee { get; set; }
        public string OffenseSelect { get; set; }
        public DateTime IssueDate {  get; set; }       
        public bool AppealSent {  get; set; }   

        public bool isPaid {  get; set; }   

    }

}

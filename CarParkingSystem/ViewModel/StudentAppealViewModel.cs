#nullable disable
namespace CarParkingSystem.ViewModel
{
    public class StudentAppealViewModel
    {
        public int Id { get; set; }
        public long OrgNumber { get; set; }
        public string RegPlate { get; set; }
        public string Severity { get; set; }   
        public string PartiesInvolved { get; set; }
        public string AdditionInformation { get; set; }
        public decimal ServerityFee { get; set; }
        public string OffenseSelect { get; set; }
        public DateTime IssueDate { get; set; }
        public string FullName {  get; set; }   
        public string Reason {  get; set; } 
        public string AppealMessage {  get; set; }  
        public bool ispaid { get; set; }    


    }
}

#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.ViewModel
{
    public class UserViewModel
    {
       
        public string FirstName { get; set; }   
        public string LastName { get; set; }
        public bool IsAccountActive { get; set; }    
        public long idNumber { get; set; }     
        public long orgNum { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string UserRole { get; set; }
        public string userId {  get; set; } 
        public string Model {  get; set; }  
        public string Make {  get; set; }   
        public string Color {  get; set; }  
        public long PermitId {  get; set; }   
        public string RegisterPlate {  get; set; }  
        public string BayId {  get; set; }  

        public string CheckInBay { get; set; }  

    }
}

#nullable disable
namespace CarParkingSystem.ViewModel
{
    public class UserInformationViewModel
    {
        public long IdNumber { get; set; }  
        public string LicenseNumber { get; set; }
        public string FirstName {  get; set; }  
        public string LastName { get; set; }    
        public bool LicenceValidity { get; set; }
    }
}

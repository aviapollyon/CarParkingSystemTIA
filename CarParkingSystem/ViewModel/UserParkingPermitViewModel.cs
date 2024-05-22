#nullable disable
namespace CarParkingSystem.ViewModel
{
    public class UserParkingPermitViewModel
    {
        public UserInformationViewModel UserInformation { get; set; }   
        public List<UserVehicleViewModel> UserVehicle { get; set;}
        public List<PermitViewModel> PermitView { get; set;}    
    }
}

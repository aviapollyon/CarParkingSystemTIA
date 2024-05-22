using CarParkingSystem.Models;

namespace CarParkingSystem.ViewModel
{
    public class ReservationPermitViewModel
    {
        public List<Permit> Permits { get; set; }   
        public List<UserReservationViewModel> Reservations { get; set; }
    }
}

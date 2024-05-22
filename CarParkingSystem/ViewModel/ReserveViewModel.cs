#nullable disable
using CarParkingSystem.Models;

namespace CarParkingSystem.ViewModel
{
    public class ReserveViewModel
    {
        public List<Reservations> Reservations { get; set; }
        public List<Reservations> UserReservations { get; set; }
        public List<ParkingBay> ParkingBays { get; set; }   
        public List<Permit> Permits { get; set; }   
        public List<Bay> Bays { get; set; } 
        public string regNumber {  get; set; }  
    }
}
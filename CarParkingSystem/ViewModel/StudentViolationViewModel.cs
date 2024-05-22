#nullable disable
using CarParkingSystem.Models;

namespace CarParkingSystem.ViewModel
{
    public class StudentViolationViewModel
    {
        public StudentViolation StudentViolation { get; set; }
        public List<ViolationImages> ViolationImages {  get; set; } 
        public ViolationAppeal violationAppeals { get; set; } 
    }
}

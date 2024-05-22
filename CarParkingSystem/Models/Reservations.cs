#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class Reservations
    {
        [Key]
        public int Id { get; set; }
        public long OrgNum { get; set; }
        public string regPlateNum { get; set; }
        public DateTime ExpDate { get; set; }
        public string BayId {  get; set; }
        public bool isGet { get; set; }
    }
}

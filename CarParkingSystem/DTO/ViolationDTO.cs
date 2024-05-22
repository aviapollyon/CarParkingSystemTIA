#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarParkingSystem.DTO
{
    public class ViolationDTO
    {
        public long OrgNumber { get; set; }
        public string RegPlate { get; set; }
        public string Severity { get; set; }
        [Required]
        public string PartiesInvolved { get; set; }
        [Required]
        public string AdditionInformation { get; set; }          
        public List<IFormFile> Listimages { get; set; }

        public decimal ServerityFee { get; set; } 
        public string OffenseSelect {  get; set; }  
    }
}

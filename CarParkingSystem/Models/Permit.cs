#nullable disable
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace CarParkingSystem.Models
{
    public class Permit
    {
        [Key]
        public int Id { get; set; } 
        public long OrgNum { get; set; }    
        public string regPlateNum { get; set; } 
        public DateTime ExpDate { get; set; }      
        public bool isGet {  get; set; }    
        public long QRCodeNumber { get; set; }

        [NotMapped]
        public string? QRCodeImage {  get; set; }

        [NotMapped]

        public string? DownloadPath {  get; set; }  
    }
}

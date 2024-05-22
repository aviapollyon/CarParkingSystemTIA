#nullable disable
using System.ComponentModel.DataAnnotations;

namespace CarParkingSystem.Models
{
    public class Bay
    {
        [Key]
        public int Id {  get; set; }
        [Required]
        [Display(Name ="Section Letter")]
        public string SectionLetter { get; set; }
        [Required]
        [Display(Name = "Bay Number")]
        public string BayNumber {  get; set; }
    }
}

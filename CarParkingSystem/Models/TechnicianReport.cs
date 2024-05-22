namespace CarParkingSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class TechnicianReport
    {
        public int Id { get; set; }

        public int FaultId { get; set; }
        public Fault Fault { get; set; }

        [Required]
        public string Report { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}

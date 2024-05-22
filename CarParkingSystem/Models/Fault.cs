namespace CarParkingSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class Fault
    {
        public int Id { get; set; }

        public int FaultReportedId { get; set; }
        public FaultReported FaultReported { get; set; }

        [Required]
        public string TechnicianId { get; set; }

        public DateTime DateAssigned { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Assigned"; // E.g., "Assigned", "In Progress", "Completed"
    }
}

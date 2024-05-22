namespace CarParkingSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;

    public class FaultReported
    {
        public int Id { get; set; }

        [Required]
        public string Description { get; set; }

        public DateTime DateReported { get; set; } = DateTime.Now;

        public string Status { get; set; } = "Pending"; // E.g., "Pending", "In Progress", "Completed"

        [Required]
        public string StudentId { get; set; }
    }
}

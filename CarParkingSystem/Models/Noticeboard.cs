namespace CarParkingSystem.Models
{
    using System;
    using System.ComponentModel.DataAnnotations;

    public class Noticeboard
    {
        public int Id { get; set; }

        [Required]
        public string Message { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.Now;
    }
}

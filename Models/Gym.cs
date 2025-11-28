using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Web.Models
{
    public class Gym
    {
        public int Id { get; set; }

        [Required]
        [StringLength(200)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [StringLength(500)]
        public string Address { get; set; } = string.Empty;

        [StringLength(100)]
        public string? WorkingHours { get; set; }

        public ICollection<Service> Services { get; set; } = new List<Service>();
        public ICollection<Trainer> Trainers { get; set; } = new List<Trainer>();
        public ICollection<Appointment> Appointments { get; set; } = new List<Appointment>();
    }
}


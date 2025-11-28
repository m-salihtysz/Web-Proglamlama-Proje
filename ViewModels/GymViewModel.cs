using System.ComponentModel.DataAnnotations;

namespace FitnessCenter.Web.ViewModels
{
    public class GymViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Spor salonu adı gereklidir")]
        [StringLength(200, ErrorMessage = "Ad 200 karakterden uzun olamaz")]
        [Display(Name = "Spor Salonu Adı")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Adres gereklidir")]
        [StringLength(500, ErrorMessage = "Adres 500 karakterden uzun olamaz")]
        [Display(Name = "Adres")]
        public string Address { get; set; } = string.Empty;

        [StringLength(100, ErrorMessage = "Çalışma saatleri 100 karakterden uzun olamaz")]
        [Display(Name = "Çalışma Saatleri")]
        public string? WorkingHours { get; set; }
    }
}


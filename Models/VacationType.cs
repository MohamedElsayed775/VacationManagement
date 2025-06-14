using System.ComponentModel.DataAnnotations;

namespace VacationManagement.Models
{
    public class VacationType : EntityBase
    {
        [StringLength(200)]
        [Display(Name = "Vacation Name")]
        public string VacationName { get; set; } = string.Empty;

        [StringLength(7)]
        [Display(Name = "Vacation Color")]
        public string backGroundColor { get; set; }=string.Empty;

        [Display(Name = "Number Days")]
        public int NumberDays { get; set; }
    }
}

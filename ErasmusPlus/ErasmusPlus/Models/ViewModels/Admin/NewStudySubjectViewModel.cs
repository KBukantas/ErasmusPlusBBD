using System.ComponentModel.DataAnnotations;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class NewStudySubjectViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public string Language { get; set; }
        [Required]
        public string Evaluation { get; set; }
        [Required]
        public decimal Credits { get; set; }
        [Required]
        public int Semester { get; set; }
    }
}
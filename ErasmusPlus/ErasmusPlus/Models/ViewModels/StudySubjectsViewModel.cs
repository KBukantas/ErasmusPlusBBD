using System.ComponentModel.DataAnnotations;

namespace ErasmusPlus.Models.ViewModels
{
    public class StudySubjectsViewModel
    {
        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string Evaluation { get; set; }

        public decimal Credits { get; set; }

        public int Semester { get; set; }
    }
}
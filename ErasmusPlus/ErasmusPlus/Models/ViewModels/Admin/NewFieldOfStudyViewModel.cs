using ErasmusPlus.Common.SharedModels;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class NewFieldOfStudyViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public Degree Degree { get; set; }

        public List<int> SelectedStudySubjects { get; set; }
        public MultiSelectList StudySubjects { get; set; }
    }
}
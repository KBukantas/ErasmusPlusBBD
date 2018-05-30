using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class NewFacultyViewModel
    {
        public int? Id { get; set; }
        [Required]
        public string Name { get; set; }
        public List<int> SelectedFos { get; set; }
        public MultiSelectList StudyFields { get; set; }
    }
}
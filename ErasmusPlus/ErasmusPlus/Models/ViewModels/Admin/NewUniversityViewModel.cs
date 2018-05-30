using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class NewUniversityViewModel
    {
        public NewUniversityViewModel()
        {
            Faculties = new SelectList(new List<KeyValueIntStr>());
            SelectedFaculties = new List<int>();
        }

        [Required]
        public string Name { get; set; }

        [Required]
        public string ErasmusCode { get; set; }

        //Should be refactored to a Country/City/Address tables
        [Required]
        public string Country { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public List<int> SelectedFaculties { get; set; }
        public SelectList Faculties { get; set; }
    }
}
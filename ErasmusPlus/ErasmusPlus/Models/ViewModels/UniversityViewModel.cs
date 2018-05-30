using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ErasmusPlus.Models.ViewModels
{
    public class UniversityViewModel
    {
        public string CountryTitle { get; set; }
        public string NameTitle { get; set; }
        public string CityTitle { get; set; }
        public string CodeTitle { get; set; }
        public string AdminTitle { get; set; }

        [StringLength(255)]
        public string Country { get; set; }

        [StringLength(255)]
        public string Name { get; set; }

        [StringLength(255)]
        public string City { get; set; }

        [StringLength(255)]
        public string Code { get; set; }

        [StringLength(255)]
        public string Admin { get; set; }

        public List<int> FacultyIds { get; set; }
    }
}
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ErasmusPlus.Models.ViewModels
{
    public class FacultyViewModel
    {
        [StringLength(100)]
        public string Name { get; set; }

        public List<int> StudyFieldIds { get; set; }
    }
}
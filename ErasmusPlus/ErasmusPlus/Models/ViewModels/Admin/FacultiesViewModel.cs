using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class FacultiesViewModel
    {
        public FacultiesViewModel()
        {
            Faculties = new List<FacultyView>();
        }

        public List<FacultyView> Faculties { get; set; }
    }

    public class FacultyView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string UniversityName { get; set; }
    }
}
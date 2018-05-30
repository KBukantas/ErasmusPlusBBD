using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class UniversitiesViewModel
    {
        public UniversitiesViewModel()
        {
            Universities = new List<UniversityView>();
        }
        public List<UniversityView> Universities { get; set; }
    }

    public class UniversityView
    {
        public int Id { get; set; }

        public string Name { get; set; }
        public string ErasmusCode { get; set; }

        //Should be refactored to a Country/City/Address tables
        public string Country { get; set; }
        public string City { get; set; }
        public string Address { get; set; }
    }
}
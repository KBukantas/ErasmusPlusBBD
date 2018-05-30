using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class UniversityAgreementsViewModel
    {
        public UniversityAgreementsViewModel()
        {
            UniversityAgreements = new List<UniversityAgreementsView>();
        }
        public List<UniversityAgreementsView> UniversityAgreements { get; set; }
    }

    public class UniversityAgreementsView
    {
        public int Id { get; set; }
        public string SourceUniversity { get; set; }
        public string TargetUniversity { get; set; }
    }
}
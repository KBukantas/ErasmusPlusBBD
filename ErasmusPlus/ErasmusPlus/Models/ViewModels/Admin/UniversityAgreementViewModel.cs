using System.Collections.Generic;
using System.Web.Mvc;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class UniversityAgreementViewModel
    {
        public UniversityAgreementViewModel()
        {
            SourceUniversityList = new SelectList(new List<KeyValueIntStr>());
            TargetUniversityList = new SelectList(new List<KeyValueIntStr>());
        }
        public string SourceUniversityName { get; set; }
        public int SourceUniversityId { get; set; }
        public SelectList SourceUniversityList { get; set; }
        public int TargetUniversityId { get; set; }
        public SelectList TargetUniversityList { get; set; }
    }
}
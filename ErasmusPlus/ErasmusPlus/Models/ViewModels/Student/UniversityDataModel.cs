using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels.Student
{
    public class UniversityDataModel
    {
        public UniversityDataModel()
        {
            TargetUniversities = new List<KeyValueIntStr>();
            SourceFaculties = new List<KeyValueIntStr>();
        }

        public List<KeyValueIntStr> TargetUniversities { get; set; }
        public List<KeyValueIntStr> SourceFaculties { get; set; }
    }

}
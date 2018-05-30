using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels
{
    public class DrillDownViewModel
    {
        public List<CountryDataList> Countries { get; set; }

        public DrillDownViewModel()
        {
            Countries = new List<CountryDataList>();
        }
    }

    public class CountryDataList
    {
        public string CountryName { get; set; }
        public List<UniversityDataList> Universities { get; set; }

        public CountryDataList()
        {
            Universities = new List<UniversityDataList>();
        }
    }

    public class UniversityDataList
    {
        public string UniversityName { get; set; }
        public int UniversityId { get; set; }
        public int CountIn { get; set; }
        public int CountOut { get; set; }
        public List<StudentInfoData> StudentsIn { get; set; }
        public List<StudentInfoData> StudentsOut { get; set; }

        public UniversityDataList()
        {
            StudentsIn = new List<StudentInfoData>();
            StudentsOut = new List<StudentInfoData>();
        }
    }

    public class StudentInfoData
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}
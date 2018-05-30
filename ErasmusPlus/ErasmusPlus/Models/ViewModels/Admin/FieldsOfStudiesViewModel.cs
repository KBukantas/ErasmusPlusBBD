using ErasmusPlus.Common.SharedModels;
using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class FieldsOfStudiesViewModel
    {
        public FieldsOfStudiesViewModel()
        {
            FieldsOfStudy = new List<FieldOfStudyView>();
        }
        public List<FieldOfStudyView> FieldsOfStudy { get; set; }

    }

    public class FieldOfStudyView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public Degree Degree { get; set; }
        public string FacultyName { get; set; }
    }
}
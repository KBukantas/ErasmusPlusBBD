using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class StudySubjectsViewModel
    {
        public StudySubjectsViewModel()
        {
            StudySubjects = new List<StudySubjectView>();
        }
        public List<StudySubjectView> StudySubjects { get; set; }
    }

    public class StudySubjectView
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Language { get; set; }
        public string Evaluation { get; set; }
        public decimal Credits { get; set; }
        public int Semester { get; set; }
        public string FieldOfStudyName { get; set; }
    }
}
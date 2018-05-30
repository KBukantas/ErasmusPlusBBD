using ErasmusPlus.Common.Database;
using ErasmusPlus.Common.SharedModels;
using ErasmusPlus.Models.Database;
using System;
using System.Collections.Generic;

namespace ErasmusPlus.Models.ViewModels.Coordinator
{
    public class StudentAgreementsViewModel
    {
        public StudentAgreementsViewModel()
        {
            StudentAgreements = new List<StudentAgreementListView>();
        }
        public List<StudentAgreementListView> StudentAgreements { get; set; }
    }

    public class StudentAgreementListView
    {
        public int Id { get; set; }
        public AgreementState State { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public ErasmusUser ErasmusUser { get; set; }
        public University SourceUniversity { get; set; }
        public University TargetUniversity { get; set; }
    }

    public class StudentAgreementDetailsView
    {
        public University SourceUniversity { get; set; }
        public University TargetUniversity { get; set; }
        public Faculty SourceFaculty { get; set; }
        public Faculty TargetFaculty { get; set; }
        public FieldOfStudy SourceFieldOfStudy { get; set; }
        public FieldOfStudy TargetFieldOfStudy { get; set; }
        public List<StudySubjects> SourceStudySubjects { get; set; }
        public List<StudySubjects> TargetStudySubjects { get; set; }

        public int Semester { get; set; }
        public string Language { get; set; }
        public LanguageLevel LanguageLevel { get; set; }
        public decimal Scholarship { get; set; }

        public string FinancingSource { get; set; }
        public string StoragePath { get; set; }
        public ErasmusUser Student { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public string State { get; set; }
    }
}
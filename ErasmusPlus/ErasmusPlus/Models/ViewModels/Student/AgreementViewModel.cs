using ErasmusPlus.Common.SharedModels;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ErasmusPlus.Models.ViewModels.Student
{
    public class AgreementViewModel
    {
        public AgreementViewModel()
        {
            SourceUniversities = new SelectList(new List<SelectListItem>());
            TargetUniversities = new SelectList(new List<SelectListItem>());
            SourceFaculties = new SelectList(new List<SelectListItem>());
            TargetFaculties = new SelectList(new List<SelectListItem>());
            SourceFieldsOfStudy = new SelectList(new List<SelectListItem>());
            TargetFieldsOfStudy = new SelectList(new List<SelectListItem>());
            SourceStudySubjects = new SelectList(new List<SelectListItem>());
            TargetStudySubjects = new SelectList(new List<SelectListItem>());
            SelectedSourceStudySubjects = new List<int>();
            SelectedTargetStudySubjects = new List<int>();
            Semesters = new SelectList(new List<SelectListItem>());
        }

        public SelectList SourceUniversities { get; set; }
        [Required]
        public int SelectedSourceUniversity { get; set; }

        public SelectList TargetUniversities { get; set; }
        [Required]
        public int SelectedTargetUniversity { get; set; }

        public SelectList SourceFaculties { get; set; }
        [Required]
        public int SelectedSourceFaculty { get; set; }

        public SelectList TargetFaculties { get; set; }
        [Required]
        public int SelectedTargetFaculty { get; set; }

        public SelectList SourceFieldsOfStudy { get; set; }
        [Required]
        public int SelectedSourceFieldOfStudy { get; set; }

        public SelectList TargetFieldsOfStudy { get; set; }
        [Required]
        public int SelectedTargetFieldOfStudy { get; set; }
        
        public MultiSelectList SourceStudySubjects { get; set; }
        [Required]
        public List<int> SelectedSourceStudySubjects { get; set; }

        public MultiSelectList TargetStudySubjects { get; set; }
        [Required]
        public List<int> SelectedTargetStudySubjects { get; set; }

        public SelectList Semesters { get; set; }
        [Required]
        public int SelectedSemester { get; set; }

        [Required]
        public string Language { get; set; }
        public LanguageLevel LanguageLevel { get; set; }

        [Required]
        public DateTime From { get; set; } = DateTime.Today;

        [Required]
        public DateTime To { get; set; } = DateTime.Today.AddDays(1);
    }

    public class StudySubjectViewModel
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int Credits { get; set; }
    }
}
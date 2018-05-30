using System;
using System.Collections.Generic;
using ErasmusPlus.Common.SharedModels;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ErasmusPlus.Models.Database;

namespace ErasmusPlus.Common.Database
{
    public class Agreement
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }
        
        public AgreementState State { get; set; }
        public string StoragePath { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime StartDate { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime EndDate { get; set; }
        
        public decimal Scholarship { get; set; }

        public string FinancingSource { get; set; }

        [ForeignKey("ErasmusUser")]
        public string ErasmusUserId { get; set; }
        public virtual ErasmusUser ErasmusUser { get; set; }

        [ForeignKey("SourceUniversity")]
        [Required]
        public int SourceUniversityId { get; set; }

        public virtual University SourceUniversity { get; set; }

        [ForeignKey("TargetUniversity")]
        [Required]
        public int TargetUniversityId { get; set; }

        public virtual University TargetUniversity { get; set; }

        [ForeignKey("SourceFaculty")]
        [Required]
        public int SourceFacultyId { get; set; }

        public virtual Faculty SourceFaculty { get; set; }

        [ForeignKey("TargetFaculty")]
        [Required]
        public int TargetFacultyId { get; set; }

        public virtual Faculty TargetFaculty { get; set; }

        [ForeignKey("SourceFieldOfStudy")]
        [Required]
        public int SourceFieldOfStudyId { get; set; }

        public virtual FieldOfStudy SourceFieldOfStudy { get; set; }

        [ForeignKey("TargetFieldOfStudy")]
        [Required]
        public int TargetFieldOfStudyId { get; set; }

        public virtual FieldOfStudy TargetFieldOfStudy { get; set; }

        public virtual ICollection<StudySubjects> SourceSubjects { get; set; }

        public virtual ICollection<StudySubjects> TargetSubjects { get; set; }

        public int StudyField { get; set; }

        public string Language { get; set; }

        public LanguageLevel LanguageLevel { get; set; }

        public int Semester { get; set; }

        public string DeclineReason { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErasmusPlus.Common.Database
{
    public class StudySubjects
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }
        public string Language { get; set; }
        public string Evaluation { get; set; }
        public decimal Credits { get; set; }
        public int Semester { get; set; }

        [ForeignKey("FieldOfStudy")]
        public int? FieldOfStudyId { get; set; }
        public virtual FieldOfStudy FieldOfStudy { get; set; }

        [ForeignKey("University")]
        public int? UniversityId { get; set; }
        public virtual University University { get; set; }

        public virtual ICollection<Agreement> SourceAgreements { get; set; }
        public virtual ICollection<Agreement> TargetAgreements { get; set; }
    }
}

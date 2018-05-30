using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ErasmusPlus.Common.SharedModels;

namespace ErasmusPlus.Common.Database
{
    public class FieldOfStudy
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        public Degree Degree { get; set; }

        [ForeignKey("Faculty")]
        public int? FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }

        [ForeignKey("University")]
        public int? UniversityId { get; set; }
        public virtual University University { get; set; }

        public virtual ICollection<StudySubjects> StudySubjects { get; set; }
    }
}

using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErasmusPlus.Common.Database
{
    public class Faculty
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public string Name { get; set; }

        [ForeignKey("University")]
        public int? UniversityId { get; set; }
        public virtual University University { get; set; }

        public virtual ICollection<FieldOfStudy> StudyFields { get; set; }
    }
}

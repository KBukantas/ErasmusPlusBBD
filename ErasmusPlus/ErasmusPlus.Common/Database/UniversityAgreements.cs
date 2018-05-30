using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ErasmusPlus.Common.Database
{
    public class UniversityAgreements
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [ForeignKey("SourceUniversity")]
        public int SourceUniversityId { get; set; }
        public virtual University SourceUniversity { get; set; }

        [ForeignKey("TargetUniversity")]
        public int TargetUniversityId { get; set; }
        public virtual University TargetUniversity { get; set; }
    }
}

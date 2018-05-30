using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using ErasmusPlus.Models.Database;

namespace ErasmusPlus.Common.Database
{
    public class UserPermissions
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        [Required]
        [ForeignKey("User")]
        public string UserId { get; set; }

        public virtual ErasmusUser User { get; set; }

        [ForeignKey("University")]
        public int UniversityId { get; set; }
        public virtual University University { get; set; }

        [ForeignKey("Faculty")]
        public int? FacultyId { get; set; }
        public virtual Faculty Faculty { get; set; }
    }
}

using System.ComponentModel.DataAnnotations;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class PermissionViewModel
    {
        
        public int Id { get; set; }
        [Required]
        public int UniversityId { get; set; }
        public string UniversityName { get; set; }
        public int? FacultyId { get; set; }
        public string FacultyName { get; set; }
    }
}
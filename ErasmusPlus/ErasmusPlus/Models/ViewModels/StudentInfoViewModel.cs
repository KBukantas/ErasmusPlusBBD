using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ErasmusPlus.Models.ViewModels
{
    public class StudentInfoViewModel
    {
        [Required]
        [MaxLength(100)]
        public string Name { get; set; }

        [Required]
        [MaxLength(100)]
        public string Surename { get; set; }

        [Required]
        public DateTime Birthday { get; set; }

        [Required]
        public string Nationality { get; set; }

        [Required]
        [DisplayName("Gender")]
        public bool IsMale { get; set; }

        [EmailAddress]
        [Required]
        public string Email { get; set; }

        [Phone]
        [Required]
        [DisplayName("Phone number")]
        public string PhoneNumber { get; set; }
    }
}
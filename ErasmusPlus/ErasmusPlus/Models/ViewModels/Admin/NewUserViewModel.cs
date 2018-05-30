using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class NewUserViewModel
    {
        public string UserId { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [DisplayName("First name")]
        //[Required]
        public string FirstName { get; set; }

        [DisplayName("Last name")]
        //[Required]
        public string LastName { get; set; }

        //[Required]
        [DisplayName("Personal code")]
        public string PersonalCode { get; set; }

        [Phone(ErrorMessage = "Must be of international format starting with a +")]
        public string Telephone { get; set; }

        public int? UniversityId { get; set; }

        [DisplayName("University")]
        public SelectList Universities { get; set; }

        public DateTime Birthday { get; set; }

        //[Required]
        public string Username { get; set; }

        [Required]
        public string Password { get; set; }

        [DisplayName("Student ID")]
        public string StudentId { get; set; }

        public string RoleName { get; set; }
    }
}
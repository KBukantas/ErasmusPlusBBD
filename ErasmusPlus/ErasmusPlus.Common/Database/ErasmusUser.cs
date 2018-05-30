using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Security.Claims;
using System.Threading.Tasks;
using ErasmusPlus.Common.Database;

namespace ErasmusPlus.Models.Database
{
    // You can add profile data for the user by adding more properties to your ErasmusUser class, please visit https://go.microsoft.com/fwlink/?LinkID=317594 to learn more.
    public class ErasmusUser : IdentityUser
    {
        public async Task<ClaimsIdentity> GenerateUserIdentityAsync(UserManager<ErasmusUser> manager)
        {
            // Note the authenticationType must match the one defined in CookieAuthenticationOptions.AuthenticationType
            var userIdentity = await manager.CreateIdentityAsync(this, DefaultAuthenticationTypes.ApplicationCookie);
            // Add custom user claims here
            userIdentity.AddClaim(new Claim("PersonalIdCode", string.IsNullOrEmpty(PersonalIdCode) ? "" : PersonalIdCode));
            userIdentity.AddClaim(new Claim("StudentId", string.IsNullOrEmpty(StudentId) ? "" : StudentId));
            userIdentity.AddClaim(new Claim("UniversityId", UniversityId.HasValue ? UniversityId.Value.ToString() : ""));

            return userIdentity;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string StudentId { get; set; }
        public string PersonalIdCode { get; set; }

        [Column(TypeName = "DateTime2")]
        public DateTime Birthday { get; set; }

        [NotMapped]
        public string FullName {get { return $"{FirstName} {LastName}"; } }

        [ForeignKey("University")]
        public int? UniversityId { get; set; }

        public virtual University University { get; set; }

        public virtual ICollection<Agreement> Agreements { get; set; }

        public virtual ICollection<UserPermissions> UserPermissions { get; set; }
    }
}
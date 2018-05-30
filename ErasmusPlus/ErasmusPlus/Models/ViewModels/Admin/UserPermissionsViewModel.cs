using System.Collections.Generic;
using System.Web.Mvc;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class UserPermissionsViewModel
    {
        public UserPermissionsViewModel()
        {
            Permissions = new List<PermissionViewModel>();
        }

        public string UserId { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }

        public List<PermissionViewModel> Permissions { get; set; }

        public SelectList UniversitiesList { get; set; }
    }
}
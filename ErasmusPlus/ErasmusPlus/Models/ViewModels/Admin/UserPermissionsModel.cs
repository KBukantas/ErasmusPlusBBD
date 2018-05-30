using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace ErasmusPlus.Models.ViewModels.Admin
{
    public class UserPermissionsModel
    {
        [Required]
        public string UserId { get; set; }

        public List<PermissionViewModel> Permissions { get; set; }
    }
}
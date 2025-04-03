using Microsoft.AspNetCore.Mvc;
using Shared.Common;
using Shared.Common.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.Request.AdminUser
{
    public class AdminUserRequestModel
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "First Name is required")]
        public string FirstName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Last Name is required")]
        public string LastName { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage ="Invaild email address")]
        [Required(ErrorMessage = "Email is required")]
        [Remote(action: "IsEmailInUse", controller: "AdminUser", areaName: null, AdditionalFields = nameof(Id), HttpMethod = "POST")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage ="User Type is required")]
        public int UserType { get; set; }

        [Required(ErrorMessage = "User Name is required")]
        public string DisplayName { get; set; } = string.Empty;

        public string? PasswordHash { get; set; }
        public string? PhoneNumber { get; set; }
        public string? EmailVerifiedToken { get; set; }
        public bool IsActive { get; set; }

        public List<ModulePermissionModel> ModulePermissions { get; set; } = new List<ModulePermissionModel>();
    }

    public class ModulePermissionModel
    {
        public int ModuleId { get; set; }
        public int UserId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }

        public List<PermissionModel> PermissionModel { get; set; } = new List<PermissionModel>();
    }
    public class PermissionModel
    {
        public int ModuleId { get; set; }
        public int PermissionId { get; set; }
        public string PermissionName { get; set; } = string.Empty;
        public bool IsOn { get; set; }
    }

    public class UserPermissions
    {
        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    }

    public class PermissionsCache
    {
        public Dictionary<string, UserPermissions> ModulePermissions { get; set; } = new Dictionary<string, UserPermissions>();
    }

    public class AdminUserPermissionRequestModel
    {
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public int PermissionId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public bool IsOn { get; set; }
    }

    public class AddUpdateAdminUserResponseModel
    {
        public int Status { get; set; }
        public int UserId { get; set; }
    }
}

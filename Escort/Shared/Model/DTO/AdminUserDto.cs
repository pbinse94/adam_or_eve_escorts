using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.Model.DTO
{
    public class AdminUserPermissionDto
    {
        public int UserId { get; set; }
        public int ModuleId { get; set; }
        public int PermissionId { get; set; }
        public string ModuleName { get; set; } = string.Empty;
        public string PermissionName { get; set; } = string.Empty;
        public bool IsOn { get; set; }


        public bool CanAdd { get; set; }
        public bool CanEdit { get; set; }
        public bool CanView { get; set; }
        public bool CanDelete { get; set; }
    } 
}

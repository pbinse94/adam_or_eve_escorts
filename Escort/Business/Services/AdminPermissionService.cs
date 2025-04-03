using Amazon.Runtime.Internal.Util;
using Microsoft.Extensions.Caching.Memory;
using Shared.Common.Enums;
using Shared.Model.Request.AdminUser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Services
{
    public class AdminPermissionService
    {
        private readonly IMemoryCache _memoryCache;
        public AdminPermissionService(IMemoryCache memoryCache)
        { _memoryCache = memoryCache; }

        public void SetUserPermissions(List<ModulePermissionModel> modulePermissions, int userId)
        {
            var permissionsCache = new PermissionsCache();

            foreach (var module in modulePermissions)
            {
                var userPermissions = new UserPermissions
                {
                    CanAdd = module.PermissionModel.Any(p => p.PermissionId == (int)PermissionTypes.Add && p.IsOn),
                    CanEdit = module.PermissionModel.Any(p => p.PermissionId == (int)PermissionTypes.Update && p.IsOn),
                    CanView = module.PermissionModel.Any(p => p.PermissionId == (int)PermissionTypes.View && p.IsOn),
                    CanDelete = module.PermissionModel.Any(p => p.PermissionId == (int)PermissionTypes.Delete && p.IsOn),
                };

                permissionsCache.ModulePermissions[module.ModuleName] = userPermissions;
            }

            // Store in memory cache
            _memoryCache.Set("UserPermissions_" + userId, permissionsCache);

        }
        public PermissionsCache GetUserPermissions(int userId)
        {
            if (_memoryCache.TryGetValue("UserPermissions_" + userId, out var cacheEntry))
            {
                return cacheEntry as PermissionsCache ?? new PermissionsCache();
            }

            return new PermissionsCache();
        }

        public bool CanAdd(string moduleName, PermissionsCache permissionsCache)
        {
            return permissionsCache?.ModulePermissions.ContainsKey(moduleName) == true && permissionsCache.ModulePermissions[moduleName].CanAdd;
        }

        public bool CanEdit(string moduleName, PermissionsCache permissionsCache)
        {
            return permissionsCache?.ModulePermissions.ContainsKey(moduleName) == true && permissionsCache.ModulePermissions[moduleName].CanEdit;
        }

        public bool CanView(string moduleName, PermissionsCache permissionsCache)
        {
            return permissionsCache?.ModulePermissions.ContainsKey(moduleName) == true && permissionsCache.ModulePermissions[moduleName].CanView;
        }

        public bool CanDelete(string moduleName, PermissionsCache permissionsCache)
        {
            return permissionsCache?.ModulePermissions.ContainsKey(moduleName) == true && permissionsCache.ModulePermissions[moduleName].CanDelete;
        }
    }
}

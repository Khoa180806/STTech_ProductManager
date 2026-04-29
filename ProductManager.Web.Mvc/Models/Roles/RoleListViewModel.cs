using ProductManager.Roles.Dto;
using System.Collections.Generic;

namespace ProductManager.Web.Models.Roles;

public class RoleListViewModel
{
    public IReadOnlyList<PermissionDto> Permissions { get; set; }
}

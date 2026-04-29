using ProductManager.Roles.Dto;
using System.Collections.Generic;

namespace ProductManager.Web.Models.Users;

public class UserListViewModel
{
    public IReadOnlyList<RoleDto> Roles { get; set; }
}

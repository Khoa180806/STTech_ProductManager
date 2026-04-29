using ProductManager.Roles.Dto;
using System.Collections.Generic;

namespace ProductManager.Web.Models.Common;

public interface IPermissionsEditViewModel
{
    List<FlatPermissionDto> Permissions { get; set; }
}
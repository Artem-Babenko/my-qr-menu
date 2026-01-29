using QrMenuAPI.Admin.Models.User;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.Admin.Mappings;

public static class UserMapper
{
    public static UserModel MapToModel(this UserEntity src) => new()
    {
        Id = src.Id,
        Name = src.Name,
        Email = src.Email,
        Surname = src.Surname,
        Phone = src.Phone,
        NetworkId = src.NetworkId,
        Accesses = src.UserEstablishment
            .Select(x => new UserEstablishmentAccessModel
            {
                EstablishmentId = x.EstablishmentId,
                RoleId = x.RoleId
            })
            .ToList()
    };
}

using QrMenuAPI.APP.Models.User;
using QrMenuAPI.Core.Entities;

namespace QrMenuAPI.APP.Mappings;

public static class UserMapper
{
    public static UserModel MapToModel(this UserEntity src) => new()
    {
        Id = src.Id,
        Name = src.Name,
        Email = src.Email,
        Surname = src.Surname,
        Phone = src.Phone,
    };
}

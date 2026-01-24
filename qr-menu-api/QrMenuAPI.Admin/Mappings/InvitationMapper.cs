using QrMenuAPI.Admin.Models.Invitation;
using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Mappings;

public static class InvitationMapper
{
    public static InvitationModel MapToModel(this InvitationEntity src)
    {
        var model = new InvitationModel()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ExpiredAt = src.ExpiredAt,
            Status = src.Status,
            RoleId = src.RoleId,
            Phone = src.Phone,
            Name = src.Name,
            Surname = src.Surname,
            EstablishmentId = src.EstablishmentId,
        };

        if (src.TargetUser != null)
        {
            model.Phone = src.TargetUser.Phone;
            model.Name = src.TargetUser.Name;
            model.Surname = src.TargetUser.Surname;
        }

        return model;
    }

    public static UserInvatationModel MapToUserModel(this InvitationEntity src)
    {
        var model = new UserInvatationModel()
        {
            Id = src.Id,
            CreatedAt = src.CreatedAt,
            ExpiredAt = src.ExpiredAt,
            Status = src.Status,
            EstablishmentId = src.EstablishmentId,
            EstablishmentAddress = src.Establishment.Address,
            EstablishmentName = src.Establishment.Name,
            NetworkId = src.Establishment.NetworkId,
            RoleId = src.RoleId,
            RoleName = src.Role.Name,
        };
        return model;
    }

    public static InvitationEntity MapToEntity(this InvitationRequest req) => new()
    {
        EstablishmentId = req.EstablishmentId,
        RoleId = req.RoleId,
        Status = InvitationStatus.Pending,
        CreatedAt = DateTime.UtcNow,
        ExpiredAt = DateTime.UtcNow.AddDays(1),
    };
}

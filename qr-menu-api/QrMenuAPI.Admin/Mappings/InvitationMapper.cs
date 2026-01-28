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

    public static UserInvitationModel MapToUserModel(this InvitationEntity src)
    {
        if (src == null)
            throw new ArgumentNullException(nameof(src));
        if (src.Establishment == null)
            throw new InvalidOperationException("Establishment cannot be null");
        if (src.Role == null)
            throw new InvalidOperationException("Role cannot be null");

        var phone = (src.Phone ?? src.TargetUser?.Phone)
            ?? throw new InvalidOperationException("Phone cannot be null");
        var name = (src.Name ?? src.TargetUser?.Name)
            ?? throw new InvalidOperationException("Name cannot be null");
        var surname = (src.Surname ?? src.TargetUser?.Surname)
            ?? throw new InvalidOperationException("Surname cannot be null");

        return new UserInvitationModel
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
            Phone = phone,
            Name = name,
            Surname = surname,
        };
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

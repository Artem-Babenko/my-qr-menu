using QrMenuAPI.Core.Entities;
using QrMenuAPI.Core.Enums;

namespace QrMenuAPI.Admin.Utils;

public static class DefaultRoles
{
    public const string Owner = "Власник";

    public static List<RoleEntity> Get()
    {
        var owner = new RoleEntity
        {
            Name = Owner,
            Description = "Повний доступ до мережі та всіх закладів.",
            Permissions = Enum.GetValues<PermissionType>()
                .Select(p => new RolePermissionEntity { PermissionType = p })
                .ToList()
        };

        var admin = new RoleEntity
        {
            Name = "Адміністратор",
            Description = "Керує закладами та персоналом, не фінансами мережі.",
            Permissions =
            [
                new() { PermissionType = PermissionType.EstablishmentsView },
                new() { PermissionType = PermissionType.EstablishmentsEdit },
                new() { PermissionType = PermissionType.UsersView },
                new() { PermissionType = PermissionType.UsersEdit },
                new() { PermissionType = PermissionType.UsersInvite },
                new() { PermissionType = PermissionType.UsersRolesEdit },
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersEdit },
                new() { PermissionType = PermissionType.OrdersChangeStatus },
                new() { PermissionType = PermissionType.MenuView },
                new() { PermissionType = PermissionType.MenuEdit },
                new() { PermissionType = PermissionType.AnalyticsView },
                new() { PermissionType = PermissionType.SettingsView },
                new() { PermissionType = PermissionType.SettingsEdit },
            ]
        };

        var manager = new RoleEntity
        {
            Name = "Менеджер",
            Description = "Керує роботою окремого закладу та звітами.",
            Permissions =
            [
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersChangeStatus },
                new() { PermissionType = PermissionType.AnalyticsView },
                new() { PermissionType = PermissionType.MenuView },
            ]
        };

        var chef = new RoleEntity
        {
            Name = "Кухар",
            Description = "Готує страви, бачить замовлення кухні.",
            Permissions =
            [
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersChangeStatus },
            ]
        };

        var waiter = new RoleEntity
        {
            Name = "Офіціант",
            Description = "Приймає замовлення та обслуговує клієнтів.",
            Permissions =
            [
                new() { PermissionType = PermissionType.MenuView },
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersEdit },
                new() { PermissionType = PermissionType.OrdersChangeStatus },
            ]
        };

        return [owner, admin, manager, chef, waiter];
    }
}

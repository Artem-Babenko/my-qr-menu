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
                new() { PermissionType = PermissionType.EstablishmentsCreate },
                new() { PermissionType = PermissionType.EstablishmentsUpdate },
                new() { PermissionType = PermissionType.EstablishmentsDelete },
                new() { PermissionType = PermissionType.NetworkEdit },
                new() { PermissionType = PermissionType.TablesView },
                new() { PermissionType = PermissionType.TablesCreate },
                new() { PermissionType = PermissionType.TablesEdit },
                new() { PermissionType = PermissionType.TablesDelete },
                new() { PermissionType = PermissionType.UsersView },
                new() { PermissionType = PermissionType.UsersEdit },
                new() { PermissionType = PermissionType.InvitationsView },
                new() { PermissionType = PermissionType.InvitationsCreate },
                new() { PermissionType = PermissionType.InvitationsDelete },
                new() { PermissionType = PermissionType.RolesView },
                new() { PermissionType = PermissionType.RolesCreate },
                new() { PermissionType = PermissionType.RolesDelete },
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersEdit },
                new() { PermissionType = PermissionType.ProductsView },
                new() { PermissionType = PermissionType.ProductsCreate },
                new() { PermissionType = PermissionType.ProductsEdit },
                new() { PermissionType = PermissionType.ProductsDelete },
                new() { PermissionType = PermissionType.CategoriesView },
                new() { PermissionType = PermissionType.CategoriesCreate },
                new() { PermissionType = PermissionType.CategoriesEdit },
                new() { PermissionType = PermissionType.CategoriesDelete },
            ]
        };

        var manager = new RoleEntity
        {
            Name = "Менеджер",
            Description = "Керує роботою окремого закладу та звітами.",
            Permissions =
            [
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersCreate },
                new() { PermissionType = PermissionType.OrdersEdit },
                new() { PermissionType = PermissionType.OrdersTakeInWork },
                new() { PermissionType = PermissionType.OrdersSendToKitchen },
                new() { PermissionType = PermissionType.OrdersStartCooking },
                new() { PermissionType = PermissionType.OrdersMarkReady },
                new() { PermissionType = PermissionType.OrdersReturn },
                new() { PermissionType = PermissionType.OrdersComplete },
                new() { PermissionType = PermissionType.OrdersCancel },
                new() { PermissionType = PermissionType.OrdersDelete },
                new() { PermissionType = PermissionType.TablesView },
                new() { PermissionType = PermissionType.ProductsView },
                new() { PermissionType = PermissionType.CategoriesView },
            ]
        };

        var chef = new RoleEntity
        {
            Name = "Кухар",
            Description = "Готує страви, бачить замовлення кухні.",
            Permissions =
            [
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersStartCooking },
                new() { PermissionType = PermissionType.OrdersMarkReady },
                new() { PermissionType = PermissionType.OrdersReturn },
            ]
        };

        var waiter = new RoleEntity
        {
            Name = "Офіціант",
            Description = "Приймає замовлення та обслуговує клієнтів.",
            Permissions =
            [
                new() { PermissionType = PermissionType.ProductsView },
                new() { PermissionType = PermissionType.CategoriesView },
                new() { PermissionType = PermissionType.TablesView },
                new() { PermissionType = PermissionType.OrdersView },
                new() { PermissionType = PermissionType.OrdersCreate },
                new() { PermissionType = PermissionType.OrdersEdit },
                new() { PermissionType = PermissionType.OrdersTakeInWork },
                new() { PermissionType = PermissionType.OrdersSendToKitchen },
                new() { PermissionType = PermissionType.OrdersComplete },
                new() { PermissionType = PermissionType.OrdersCancel },
            ]
        };

        return [owner, admin, manager, chef, waiter];
    }
}

namespace QrMenuAPI.Core.Enums;

public enum PermissionType
{
    OrdersView = 1,
    OrdersCreate,
    OrdersEdit,
    OrdersTakeInWork,
    OrdersSendToKitchen,
    OrdersStartCooking,
    OrdersMarkReady,
    OrdersReturn,
    OrdersComplete,
    OrdersCancel,
    OrdersDelete,

    UsersView,
    UsersEdit,
    UsersDelete,

    InvitationsView,
    InvitationsCreate,
    InvitationsDelete,

    RolesView,
    RolesCreate,
    RolesDelete,

    EstablishmentsCreate,
    EstablishmentsUpdate,
    EstablishmentsDelete,

    NetworkEdit,

    TablesView,
    TablesCreate,
    TablesEdit,
    TablesDelete,

    ProductsView,
    ProductsCreate,
    ProductsEdit,
    ProductsDelete,

    CategoriesView,
    CategoriesCreate,
    CategoriesEdit,
    CategoriesDelete,
}

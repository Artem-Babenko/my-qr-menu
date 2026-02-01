namespace QrMenuAPI.Admin.Consts;

public static class ErrorCodes
{
    public const string InvalidRequest = "invalid_request";
    public const string InvalidCredentials = "invalid_credentials";
    public const string DuplicatePhone = "duplicate_phone";
    public const string InvitationNotFound = "invitation_not_found";
    public const string UserNotFound = "user_not_found";
    public const string PermissionDenied = "permission_denied";
    public const string DuplicateEstablishment = "duplicate_establishment";
    public const string DuplicateNetwork = "duplicate_network";
    public const string NetworkNotFound = "network_not_found";
    public const string EstablishmentNotFound = "establishment_not_found";
    public const string RoleNotFound = "role_not_found";
    public const string RoleDuplicateName = "role_duplicate_name";
    public const string RoleHasUsers = "role_has_users";
    public const string RoleDeleteForbidden = "role_delete_forbidden";

    public const string TableNotFound = "table_not_found";
    public const string DuplicateTableNumber = "duplicate_table_number";

    public const string CategoryNotFound = "category_not_found";
    public const string DuplicateCategoryName = "duplicate_category_name";
    public const string CategoryDeleteForbidden = "category_delete_forbidden";

    public const string ProductNotFound = "product_not_found";
    public const string DuplicateProductName = "duplicate_product_name";

    public const string OrderNotFound = "order_not_found";
    public const string OrderInvalidStatus = "order_invalid_status";
    public const string OrderItemsRequired = "order_items_required";
    public const string OrderDeleteForbidden = "order_delete_forbidden";
    public const string ProductNotAvailable = "product_not_available";
}

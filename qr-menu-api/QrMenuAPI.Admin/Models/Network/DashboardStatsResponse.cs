namespace QrMenuAPI.Admin.Models.Network;

public class DashboardStatsResponse
{
    public int ActiveOrdersCount { get; set; }
    public int ActiveUsersCount { get; set; }
    public int CompletedTodayOrdersCount { get; set; }
    public decimal CompletedTodayTotalSum { get; set; }
}

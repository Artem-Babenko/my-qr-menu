namespace QrMenuAPI.Admin.Models.Analytics;

public class AnalyticsOverviewResponse
{
    public DateTime DateFrom { get; set; }
    public DateTime DateTo { get; set; }
    public decimal TotalSales { get; set; }
    public int TotalOrders { get; set; }
    public decimal AverageCheck { get; set; }
    public double AverageTimeMinutes { get; set; }
    public List<AnalyticsPointResponse> SalesByHours { get; set; } = [];
    public List<AnalyticsPointResponse> OrdersByHours { get; set; } = [];
    public List<AnalyticsPointResponse> SalesByDays { get; set; } = [];
    public List<AnalyticsPointResponse> OrdersByDays { get; set; } = [];
    public List<PopularDishResponse> PopularDishes { get; set; } = [];
}

public class AnalyticsPointResponse
{
    public string Key { get; set; } = string.Empty;
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
}

public class PopularDishResponse
{
    public string Name { get; set; } = string.Empty;
    public int OrdersCount { get; set; }
    public decimal TotalSum { get; set; }
}

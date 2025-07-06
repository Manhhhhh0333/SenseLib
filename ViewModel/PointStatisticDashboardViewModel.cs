public class PointStatisticDashboardViewModel
{
    public int TotalPointsInSystem { get; set; }
    public int TotalPointsEarned { get; set; }
    public int TotalPointsSpent { get; set; }

    public List<UserPointStatViewModel> UserStats { get; set; }
    public List<DocumentPointStatViewModel> DocumentStats { get; set; }
}
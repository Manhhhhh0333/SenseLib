namespace DOAN.ViewModels
{
    public class RatingStatisticViewModel
    {
        public int TotalRatings { get; set; }

        public List<RatingByDocument> RatingsByDocument { get; set; }

        public List<RecentRating> RecentRatings { get; set; }

        public class RatingByDocument
        {
            public string DocumentTitle { get; set; }
            public double AverageRating { get; set; }
            public int RatingCount { get; set; }
        }

        public class RecentRating
        {
            public int RatingID { get; set; }
            public string UserName { get; set; }
            public string DocumentTitle { get; set; }
            public int RatingValue { get; set; }
            public DateTime CreatedAt { get; set; }
        }
    }
}

namespace DOAN.ViewModels
{
    public class CommentStatisticViewModel
    {
        public int TotalComments { get; set; }
        public double AverageRating { get; set; }

        public List<CommentByUser> CommentsByUser { get; set; }
        public List<CommentDetail> RecentComments { get; set; }

        public class CommentByUser
        {
            public string Email { get; set; }
            public int Count { get; set; }
        }

        public class CommentDetail
        {
            public int Id { get; set; }
            public string Content { get; set; }
            public string UserEmail { get; set; }
            public DateTime? CreatedAt { get; set; }
        }
    }
}

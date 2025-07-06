namespace DOAN.ViewModels
{
    public class FavoriteStatisticViewModel
    {
        public int TotalFavorites { get; set; }

        public List<DocumentFavorite> TopFavoritedDocuments { get; set; }

        public class DocumentFavorite
        {
            public string DocumentTitle { get; set; }
            public int FavoriteCount { get; set; }
            public List<string> UserEmails { get; set; }
        }
    }
}

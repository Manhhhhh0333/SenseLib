namespace DOAN.ViewModel
{
    public class DownloadHistoryViewModel
    {
        public int DocumentID { get; set; }
        public string DocumentName { get; set; }
        public string UserName { get; set; }
        public DateTime? DownloadDate { get; set; }
        public int? RatedStar { get; set; }
    }
}

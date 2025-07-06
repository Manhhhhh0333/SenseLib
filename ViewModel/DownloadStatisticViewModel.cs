namespace DOAN.ViewModels
{
    public class DownloadStatisticViewModel
    {
        public int TotalDownloads { get; set; }

        public List<DownloadByDocument> DownloadsByDocument { get; set; }

        public class DownloadByDocument
        {
            public string DocumentTitle { get; set; }
            public int DownloadCount { get; set; }
        }
    }
}

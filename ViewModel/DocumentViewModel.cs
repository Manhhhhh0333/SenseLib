namespace DOAN.ViewModel
{
    public class DocumentViewModel
    {
        public int DocumentID { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public string? FileType { get; set; }
        public string? FilePath { get; set; }
        public string? Status { get; set; }
        public int CategoryID { get; set; }
        public int PublisherID { get; set; }
        public int AuthorID { get; set; }
        public string? AuthorName { get; set; }
        public int? UploadedBy { get; set; }
        public string? Image { get; set; }
        public DateTime? UploadDate { get; set; }

        public double? AverageStar { get; set; }

        public int? DocumentPoint { get; set; }
    }
}

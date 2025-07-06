public class DocumentPointStatViewModel
{
    public int DocumentID { get; set; }
    public string? Title { get; set; }
    public int? UploadedBy { get; set; }
    public int TotalDownloads { get; set; }
    public int TotalPointsEarned { get; set; }
}
using Microsoft.AspNetCore.Mvc.Rendering;

namespace DOAN.ViewModel
{
    public class UploadDocumentVM
    {
        public string Title { get; set; }
        public string? Description { get; set; }
        public int AuthorID { get; set; }
        public int CategoryID { get; set; }
        public int PublisherID { get; set; }
        public string? Image { get; set; }
        public string? FilePath { get; set; }
        public int? DocumentPoint { get; set; }
        public List<SelectListItem> Authors { get; set; }
        public List<SelectListItem> Categories { get; set; }
        public List<SelectListItem> Publishers { get; set; }
    }
}

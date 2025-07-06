using DOAN.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class tblRating
{
    [Key]
    public int RatingID { get; set; }

    public int UserID { get; set; }

    public int DocumentID { get; set; }

    public int RatingValue { get; set; }

    public DateTime? CreatedAt { get; set; }

    [ForeignKey("DocumentID")]
    public tblDocuments? Document { get; set; }

}

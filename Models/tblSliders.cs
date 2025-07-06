using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DOAN.Models
{
	[Table("Sliders")]
	public class tblSliders
	{
		[Key]
		public int SliderID { get; set; }
		public string? ImagePath { get; set; }
		public string? Caption { get; set; }
		public string? Link { get; set; }
		public bool? Status { get; set; }

	}
}

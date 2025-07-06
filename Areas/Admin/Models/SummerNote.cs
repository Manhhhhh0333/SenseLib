using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
namespace DOAN.Areas.Admin.Models
{
    public class SummerNote
    {
        public SummerNote(string idEditor, bool loadLibrary = true)
        {
            IDEditor = idEditor;
            LoadLibrary = loadLibrary;
        }
        public string IDEditor { get; set; }
        public bool LoadLibrary { get; set; }

        public int Height { set; get; } = 500;

        public string toolBar { set; get; } = @"
            [
                ['style', ['style']],
                ['font', ['bold', 'underline', 'clear']],
                ['color', ['color']],
                ['para', ['ul', 'ol', 'paragraph']],
                ['table', ['table']],
                ['insert', ['link', 'elfinderFiles', 'video', 'elfinder']],
                ['view', ['fullscreen', 'codeview', 'help']]
            ]
        ";
    }
}

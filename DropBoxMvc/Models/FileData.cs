using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DropBoxMvc.Models
{
    public class FileData
    {
        public string FileName { get; set; }
        public string FileSize { get; set; }
        public string ModifiedOn { get; set; }
    }
}

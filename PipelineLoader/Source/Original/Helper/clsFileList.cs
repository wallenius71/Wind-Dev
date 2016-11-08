using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Helper
{
    public class clsFileList
    {
        public string Filename { get; set; }
        public string Directory { get; set; }
        public int FileLocationId { get; set; }
        public int FileFormatId { get; set; }
        public int SiteId { get; set; }
        public bool Encrypted { get; set; }
        public string Encoding { get; set; }
        public int ProcessFileId { get; set; }
        public string OriginalFullFilepath { get; set; }

        public string FullFilePath
        {
            get
            {
                return Directory + @"\" + Filename;
            }

            set
            {
                if(value.Contains(@"\"))
                {
                    Directory = value.Substring(0, value.LastIndexOf(@"\"));
                    Filename = value.Substring(value.LastIndexOf(@"\")+1);
                }
            }
        }

        public bool IsExcel()
        {
            return Filename.ToLower().EndsWith(".xls") || Filename.ToLower().EndsWith(".xlsx");
        }

        public string FileFormatIdPadded()
        {
            return FileFormatId.ToString().PadLeft(4, '0');
        }

    }
}

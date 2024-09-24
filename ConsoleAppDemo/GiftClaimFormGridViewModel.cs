using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDemo
{
    public class GiftClaimFormGridViewModel
    {
        public List<FileBaseModel> SerialNumberFiles { get; set; }
    }

    public class FileBaseModel
    {
        public FileBaseModel(string url, string fileName)
        {
            Url = url;
            FileName = fileName;
        }
        public string Url { get; set; }
        public string FileName { get; set; }
    }
}

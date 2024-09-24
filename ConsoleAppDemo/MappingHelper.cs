using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleAppDemo
{
    public static class MappingHelper
    {
        public static List<FileBaseModel> MapSerialNumberFiles(string serialNumberFiles)
        {
            if (string.IsNullOrEmpty(serialNumberFiles)) return new List<FileBaseModel>();
         
            return JsonConvert.DeserializeObject<List<FileBaseModel>>(serialNumberFiles) ?? new List<FileBaseModel>();
        }

        public static string GenerateString(List<FileBaseModel> models)
        {
            return JsonConvert.SerializeObject(models);
        }
    }
}

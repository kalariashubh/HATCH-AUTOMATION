using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace hatch_automation.Services
{
    public static class JsonService
    {
        public static void Export(List<LineData> lines)
        {
            string folder = @"D:\Buniyad Byte\POC 2\HATCH AUTOMATION";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, "line_lengths.json");

            string json =
                JsonConvert.SerializeObject(lines, Formatting.Indented);

            File.WriteAllText(path, json);
        }
    }
}

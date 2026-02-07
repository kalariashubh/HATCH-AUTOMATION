using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace hatch_automation.Services
{
    public class AggregatedLineData
    {
        public double Length { get; set; }
        public int Quantity { get; set; }
    }

    public class DirectionalOutput
    {
        public List<AggregatedLineData> Horizontal { get; set; }
        public List<AggregatedLineData> Vertical { get; set; }
    }

    public static class JsonService
    {
        public static void Export(List<LineData> horizontal, List<LineData> vertical)
        {
            string folder = @"D:\Buniyad Byte\POC 2\HATCH AUTOMATION";

            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            string path = Path.Combine(folder, "line_lengths.json");

            var output = new DirectionalOutput
            {
                Horizontal = Aggregate(horizontal),
                Vertical = Aggregate(vertical)
            };

            string json =
                JsonConvert.SerializeObject(output, Formatting.Indented);

            File.WriteAllText(path, json);
        }

        private static List<AggregatedLineData> Aggregate(List<LineData> lines)
        {
            if (lines == null || lines.Count == 0)
                return null;

            return lines
                .GroupBy(l => System.Math.Round(l.Length, 2))
                .Select(g => new AggregatedLineData
                {
                    Length = g.Key,
                    Quantity = g.Count()
                })
                .OrderBy(x => x.Length)
                .ToList();
        }
    }
}

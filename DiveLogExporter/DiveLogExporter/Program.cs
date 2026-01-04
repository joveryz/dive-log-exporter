using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using DiveLogExporter.Exporter;
using DiveLogExporter.Model;

namespace DiveLogExporter
{
    internal class Program
    {
        static int Main(string[] args)
        {
            // 1st arg: input directory with dive log files
            // 2nd arg: output directory
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: DiveLogExporter <input directory with dive log files> <output directory>");
                return 1;
            }

            var diveLogs = new List<GeneralDiveLog>();
            var factory = new ExporterFactory();

            // enumerate all files in the input directory, process each with the appropriate exporter
            // only process files at the top level, do not recurse into subdirectories
            Directory.EnumerateFiles(args[0]).ToList().ForEach(file =>
            {
                var exporter = factory.GetExporter(file);
                if (exporter != null)
                {
                    Console.WriteLine($"[Main] Processing file: {file} with exporter: {exporter.GetType().Name}");
                    var exportedDiveLogs = exporter.Export(file);
                    diveLogs.AddRange(exportedDiveLogs);
                    Console.WriteLine($"[Main] Exported {exportedDiveLogs.Count} dive logs from file: {file}");
                }
            });

            Console.WriteLine($"[Main] Total dive logs exported: {diveLogs.Count}");

            var allSummaries = new StringBuilder();
            var allTanks = new StringBuilder();
            var allSamples = new StringBuilder();
            allSummaries.AppendLine(new GeneralDiveLogSummary().ToCsvHeader());
            allTanks.AppendLine(new GeneralDiveLogTankInformation().ToCsvHeader());
            allSamples.AppendLine(new GeneralDiveLogSample().ToCsvHeader());

            var outputPath = args[1];
            if (!outputPath.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                outputPath += Path.DirectorySeparatorChar;
            }

            Console.WriteLine($"[Main] Exporting dive logs to {outputPath}");
            foreach (var diveLog in diveLogs)
            {
                var summary = diveLog.Summary.ToCsvRow();
                var tanks = diveLog.Tanks.ToCsvRows();
                var samples = diveLog.Samples.ToCsvRows();

                if (!string.IsNullOrWhiteSpace(summary))
                {
                    allSummaries.AppendLine(summary);
                }

                if (!string.IsNullOrWhiteSpace(tanks))
                {
                    allTanks.AppendLine(tanks);
                }

                if (!string.IsNullOrWhiteSpace(samples))
                {
                    allSamples.AppendLine(samples);
                }
            }

            File.WriteAllText(Path.Combine(outputPath, "general-dive-log-summaries.csv"), allSummaries.ToString());
            File.WriteAllText(Path.Combine(outputPath, "general-dive-log-tanks.csv"), allTanks.ToString());
            File.WriteAllText(Path.Combine(outputPath, "general-dive-log-samples.csv"), allSamples.ToString());
            Console.WriteLine("[Main] Export complete");

            return 0;
        }
    }
}

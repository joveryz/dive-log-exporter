using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using DiveLogExporter.Model;
using DiveLogExporter.Parser;

namespace DiveLogExporter
{
    public class Program
    {
        private static ProgramVersion ExporterVersion { get; set; }

        static int Main(string[] args)
        {
            ExporterVersion = new ProgramVersion();
            GetVersion();

            // Handle --version argument
            if (args.Length == 1 && (args[0] == "--version" || args[0] == "-v"))
            {
                Console.WriteLine(ExporterVersion.ToString());
                return 0;
            }

            // 1st arg: input directory with dive log files
            // 2nd arg: output directory
            if (args.Length != 1 && args.Length != 2)
            {
                Console.WriteLine("Usage: DiveLogExporter <input directory> <output directory>");
                Console.WriteLine("   or: DiveLogExporter <directory>");
                Console.WriteLine("   or: DiveLogExporter --version");
                return 1;
            }

            var inputPath = args[0];
            var outputPath = args.Length == 2 ? args[1] : args[0];

            var diveLogs = new List<GeneralDiveLog>();
            var factory = new ParserFactory();

            // enumerate all files in the input directory, process each with the appropriate exporter
            // only process files at the top level, do not recurse into subdirectories
            Directory.EnumerateFiles(inputPath).ToList().ForEach(file =>
            {
                var parser = factory.GetParser(file);
                if (parser != null)
                {
                    Console.WriteLine($"[Main] Processing file: {file} with parser: {parser.GetType().Name}");
                    var parsedDiveLogs = parser.Parse(file);
                    diveLogs.AddRange(parsedDiveLogs);
                    Console.WriteLine($"[Main] Parsed {parsedDiveLogs.Count} dive logs from file: {file}");
                }
            });

            AdjustDiveLogNumbers(ref diveLogs);
            ExportDiveLogsToCsvFiles(diveLogs, outputPath);

            Console.WriteLine($"[Main] Export complete, total dive logs parsed: {diveLogs.Count}");
            return 0;
        }

        private static void AdjustDiveLogNumbers(ref List<GeneralDiveLog> diveLogs)
        {
            diveLogs = diveLogs.OrderBy(diveLog => diveLog.Summary.StartDate).ToList();

            int currentNumber = 1;
            foreach (var diveLog in diveLogs)
            {
                if (diveLog.Summary.Number != currentNumber)
                {
                    Console.WriteLine($"[Main] Adjusting dive log number from {diveLog.Summary.Number} to {currentNumber}, diff: {currentNumber-diveLog.Summary.Number}");
                }

                diveLog.Summary.Number = currentNumber;
                if (diveLog.Tanks != null)
                {
                    foreach (var tank in diveLog.Tanks)
                    {
                        tank.Number = currentNumber;
                    }
                }
                if (diveLog.Samples != null)
                {
                    foreach (var sample in diveLog.Samples)
                    {
                        sample.Number = currentNumber;
                    }
                }

                currentNumber++;
            }

            diveLogs = diveLogs.OrderBy(diveLog => diveLog.Summary.Number.Value).ToList();
        }

        private static void ExportDiveLogsToCsvFiles(List<GeneralDiveLog> diveLogs, string outputPath)
        {
            var allSummaries = new StringBuilder();
            var allTanks = new StringBuilder();
            var allSamples = new StringBuilder();
            var version = new StringBuilder();
            allSummaries.AppendLine(new GeneralDiveLogSummary().ToCsvHeader());
            allTanks.AppendLine(new GeneralDiveLogTankInformation().ToCsvHeader());
            allSamples.AppendLine(new GeneralDiveLogSample().ToCsvHeader());
            version.AppendLine(new ProgramVersion().ToCsvHeader());
            version.AppendLine(ExporterVersion.ToCsvRow());

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
            File.WriteAllText(Path.Combine(outputPath, "general-dive-log-exporter-version.csv"), version.ToString());
        }

        private static void GetVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            ExporterVersion.Version = assembly.GetName().Version.ToString();

            // Get git commit from assembly metadata
            var metadataAttributes = assembly.GetCustomAttributes<AssemblyMetadataAttribute>();
            foreach (var attr in metadataAttributes)
            {
                if (attr.Key == "GitCommit")
                {
                    ExporterVersion.Commit = attr.Value;
                }
                else if (attr.Key == "BuildDateUtc")
                {
                    ExporterVersion.BuildDate = attr.Value;
                }
            }
        }

        private class ProgramVersion
        {
            public string Version { get; set; }

            public string Commit { get; set; }

            public string BuildDate { get; set; }

            public override string ToString()
            {
                return $"v{Version}({Commit}) on {BuildDate}";
            }
        }
    }
}

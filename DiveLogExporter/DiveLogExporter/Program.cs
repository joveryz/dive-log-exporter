using System;
using System.IO;
using System.Text;
using Assets.Scripts.Persistence.LocalCache;
using ExtendedCoreParserUtilities;

namespace DiveLogExporter
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // 1st arg: path to shearwater db
            // 2nd arg: path to output csv files
            if (args.Length != 2)
            {
                Console.WriteLine("Usage: DiveLogExporter <path to shearwater db> <output directory>");
                return;
            }

            var shearwaterDataService = new DataService(args[0]);
            var shearwaterDiveLogs = shearwaterDataService.GetDiveLogsWithRaw();
            Console.WriteLine($"Found {shearwaterDiveLogs.Count} dives in Shearwater database.");

            var summaryCsvString = new StringBuilder();
            var tankCsvString = new StringBuilder();
            var samplesCsvString = new StringBuilder();


            summaryCsvString.AppendLine(new ExportedDiveLogSummary().ToCsvHeader());
            tankCsvString.AppendLine(new ExportedDiveLogTank().ToCsvHeader());
            samplesCsvString.AppendLine(new ExportedDiveLogSample().ToCsvHeader());

            foreach (var shearwaterDiveLog in shearwaterDiveLogs)
            {
                var shearwaterDiveLogSamples = shearwaterDataService.GetDiveLogRecordsWithRaw(shearwaterDiveLog.DiveID);
                var exportedDiveLog = new ExportedDiveLog(shearwaterDiveLog, shearwaterDiveLogSamples);
                summaryCsvString.AppendLine(exportedDiveLog.Summary.ToCsvRow());
                tankCsvString.AppendLine(exportedDiveLog.Tank.ToCsvRow());
                samplesCsvString.AppendLine(exportedDiveLog.Samples.ToCsvRows());
            }
            var destDir = args[1];
            if (!destDir.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                destDir += Path.DirectorySeparatorChar;
            }
            Console.WriteLine($"Writing export files to {destDir}...");
            File.WriteAllText(Path.Combine(destDir, "shearwater-export-summary.csv"), summaryCsvString.ToString());
            File.WriteAllText(Path.Combine(destDir, "shearwater-export-tanks.csv"), tankCsvString.ToString());
            File.WriteAllText(Path.Combine(destDir, "shearwater-export-samples.csv"), samplesCsvString.ToString());
            Console.WriteLine("Export complete.");
        }
    }
}

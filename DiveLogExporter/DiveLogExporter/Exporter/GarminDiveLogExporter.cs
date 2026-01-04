using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiveLogExporter.Garmin;
using DiveLogExporter.Model;
using Dynastream.Fit;

namespace DiveLogExporter.Exporter
{
    public class GarminDiveLogExporter : IDiveLogExporter
    {
        public string Name => "Garmin";

        public IReadOnlyList<string> SupportedExtensions => new List<string> { ".fit" };

        public bool CanHandle(string inputPath)
        {
            return System.IO.File.Exists(inputPath) && SupportedExtensions.Contains(Path.GetExtension(inputPath), StringComparer.OrdinalIgnoreCase);
        }

        public List<GeneralDiveLog> Export(string inputPath)
        {
            var decodeDemo = new Decode();
            var fitListener = new FitListener();
            decodeDemo.MesgEvent += fitListener.OnMesg;
            decodeDemo.Read(new FileStream(inputPath, FileMode.Open));

            Console.WriteLine($"[{Name}] Found {fitListener.FitMessages.LapMesgs.Count} dives");
            return ExportDiveLogs(fitListener.FitMessages);
        }

        private List<GeneralDiveLog> ExportDiveLogs(FitMessages garminDiveLogs)
        {
            var res = new List<GeneralDiveLog>();
            var summaries = new List<GeneralDiveLogSummary>();

            var garminSession = garminDiveLogs.SessionMesgs.First();
            var garminDiveSettings = garminDiveLogs.DiveSettingsMesgs.First();
            var garminDeviceInfo = garminDiveLogs.DeviceInfoMesgs.First();

            for (int i = 0; i < garminDiveLogs.LapMesgs.Count; ++i)
            {
                var garminLap = garminDiveLogs.LapMesgs[i];
                var garminDiveSummary = garminDiveLogs.DiveSummaryMesgs[i + 1];

                summaries.Add(new GeneralDiveLogSummary
                {
                    // Summary Info
                    Number = (int)garminDiveSummary.GetReferenceIndex(),
                    Mode = GarminUtils.GetDiveMode(garminLap),
                    StartDate = new Dynastream.Fit.DateTime(garminLap.GetStartTime().GetTimeStamp(), i == 0 ? 0 : -3).ToString(),
                    EndDate = new Dynastream.Fit.DateTime(garminLap.GetStartTime().GetTimeStamp(), garminDiveSummary.GetBottomTime().Value + 2).ToString(),
                    DurationInSeconds = (int)Math.Floor(garminDiveSummary.GetBottomTime().Value) + (i == 0 ? 2 : 5),
                    Buddy = "Tongbo",
                    Location = "Beijing",
                    Site = "HiDive",
                    //Note = "Unknown",

                    // Environment Info
                    DepthInMetersMax = garminDiveSummary.GetMaxDepth().Value,
                    DepthInMetersAvg = garminDiveSummary.GetAvgDepth().Value,
                    HeartRateMax = garminLap.GetMaxHeartRate().Value,
                    HeartRateMin = garminLap.GetMinHeartRate().Value,
                    HeartRateAvg = garminLap.GetAvgHeartRate().Value,
                    TemperatureInCelsiusMax = garminLap.GetMaxTemperature().Value,
                    TemperatureInCelsiusMin = garminLap.GetMinTemperature().Value,
                    TemperatureInCelsiusAvg = garminLap.GetAvgTemperature().Value,
                    SurfacePressureInMillibarPreDive = -1,
                    SurfacePressureInMillibarPostDive = -1,
                    SurfaceIntervalInSeconds = (int)garminDiveSummary.GetSurfaceInterval().GetValueOrDefault(0),
                    WaterDenisity = (int)Math.Floor(garminDiveSettings.GetWaterDensity().Value),
                    WaterType = garminDiveSettings.GetWaterType().ToString(),

                    // Computer Info
                    ComputerModel = GarminUtils.GetComputerName(garminDeviceInfo),
                    ComputerSerialNumber = garminDeviceInfo.GetSerialNumber().ToString(),
                    ComputerFirmwareVersion = garminDeviceInfo.GetSoftwareVersion().ToString(),
                    BatteryType = "Li-Ion",
                    //BatteryVoltagePreDive = -1,
                    //BatteryVoltagePostDive = -1,
                    SampleRateInMs = 1000,
                    DataFormat = $"fit",
                });

                res.Add(new GeneralDiveLog
                {
                    Summary = summaries.Last(),
                });
            }

            foreach (var garminRecord in garminDiveLogs.RecordMesgs)
            {
                var currentTime = garminRecord.GetTimestamp().GetDateTime();

                foreach (var diveLog in res)
                {
                    var diveStartTime = System.DateTime.Parse(diveLog.Summary.StartDate);
                    var diveEndTime = System.DateTime.Parse(diveLog.Summary.EndDate);

                    if (currentTime >= diveStartTime && currentTime <= diveEndTime)
                    {
                        var sample = new GeneralDiveLogSample
                        {
                            Number = diveLog.Summary.Number,
                            ElapsedTimeInSeconds = (int)(currentTime - diveStartTime).TotalSeconds,
                            Depth = garminRecord.GetDepth(),
                            Temperature = (int)garminRecord.GetTemperature(),
                            HeartRate = (int)garminRecord.GetHeartRate(),
                        };

                        if (diveLog.Samples == null)
                        {
                            diveLog.Summary.SurfacePressureInMillibarPreDive = (int)garminRecord.GetAbsolutePressure().Value / 100;
                            diveLog.Samples = new List<GeneralDiveLogSample>();
                        }
                        diveLog.Summary.SurfacePressureInMillibarPostDive = (int)garminRecord.GetAbsolutePressure().Value / 100;
                        diveLog.Samples.Add(sample);
                    }
                }
            }

            return res;
        }
    }
}

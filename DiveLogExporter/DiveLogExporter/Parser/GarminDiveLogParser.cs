using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiveLogExporter.Garmin;
using DiveLogExporter.Model;
using Dynastream.Fit;

namespace DiveLogExporter.Parser
{
    public class GarminDiveLogParser : IDiveLogParser
    {
        public string Name => "Garmin";

        public IReadOnlyList<string> SupportedExtensions => new List<string> { ".fit" };

        public bool CanHandle(string inputPath)
        {
            return System.IO.File.Exists(inputPath) && SupportedExtensions.Contains(Path.GetExtension(inputPath), StringComparer.OrdinalIgnoreCase);
        }

        public List<GeneralDiveLog> Parse(string inputPath)
        {
            var decodeDemo = new Decode();
            var fitListener = new FitListener();
            var splits = Path.GetFileNameWithoutExtension(inputPath).Split('_');
            var buddy = splits.Length >= 2 ? splits[1] : "Solo";
            var location = splits.Length >= 3 ? splits[2] : "Unknown";
            var site = splits.Length >= 4 ? splits[3] : "Unknown";
            var diver = splits.Length >= 5 ? splits[4] : "Unknown";
            var note = splits.Length >= 6 ? splits[5] : null;

            decodeDemo.MesgEvent += fitListener.OnMesg;
            decodeDemo.Read(System.IO.File.OpenRead(inputPath));

            Console.WriteLine($"[{Name}] Found {fitListener.FitMessages.LapMesgs.Count} dives");
            return ParseDiveLogs(fitListener.FitMessages, diver, buddy, location, site, note);
        }

        private List<GeneralDiveLog> ParseDiveLogs(FitMessages garminDiveLogs, string diver, string buddy, string location, string site, string note)
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
                    StartDate = new Dynastream.Fit.DateTime(garminLap.GetStartTime().GetTimeStamp(), i == 0 ? 0 : -3).GetDateTime(), // Garmin use UTC time by default
                    EndDate = new Dynastream.Fit.DateTime(garminLap.GetStartTime().GetTimeStamp(), garminDiveSummary.GetBottomTime().Value + 2).GetDateTime(), // Garmin use UTC time by default
                    DurationInSeconds = (int)Math.Floor(garminDiveSummary.GetBottomTime().Value) + (i == 0 ? 2 : 5),
                    Diver = diver,
                    Buddy = buddy,
                    Location = location,
                    Site = site,
                    Note = note,

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
                    var diveStartTime = diveLog.Summary.StartDate.Value;
                    var diveEndTime = diveLog.Summary.EndDate.Value;

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

using System;
using System.Collections.Generic;
using DiveLogModels;
using Shearwater;

namespace DiveLogExporter
{
    internal class ExportedDiveLog
    {
        public ExportedDiveLogSummary Summary { get; set; }

        public ExportedDiveLogTank Tank { get; set; }

        public List<ExportedDiveLogSample> Samples { get; set; }

        public ExportedDiveLog(DiveLog shearwaterDiveLog, List<DiveLogSample> shearwaterDiveLogSamples)
        {
            var number = ShearwaterUtilsWrapper.GetDiveNumber(shearwaterDiveLog);

            var header = shearwaterDiveLog.DiveLogHeader;
            var footer = shearwaterDiveLog.DiveLogFooter;
            var details = shearwaterDiveLog.DiveLogDetails;
            var interpretedLog = shearwaterDiveLog.InterpretedLogData;

            Summary = new ExportedDiveLogSummary
            {
                // Summary Info
                Number = number,
                Mode = ShearwaterUtilsWrapper.GetDiveMode(shearwaterDiveLog),
                StartDate = details.DiveDate.ToString(),
                EndDate = details.DiveDate.AddSeconds(footer.DiveTimeInSeconds).ToString(),
                DurationInSeconds = footer.DiveTimeInSeconds,
                Buddy = details.Buddy.Value,
                Location = details.Location.Value,
                Site = details.Site.Value,
                Note = details.Notes.Value,

                // Environment Info
                DepthInMetersMax = footer.MaxDiveDepth,
                DepthInMetersAvg = interpretedLog.AverageDepth,
                TemperatureInCelsiusMax = interpretedLog.MaxTemp,
                TemperatureInCelsiusMin = interpretedLog.MinTemp,
                TemperatureInCelsiusAvg = interpretedLog.AverageTemp,
                SurfacePressureInMillibarPreDive = header.SurfacePressure,
                SurfacePressureInMillibarPostDive = footer.SurfacePressure,
                SurfaceIntervalInSeconds = (int)TimeSpan.FromMinutes(header.SurfaceTime).TotalSeconds,
                Salinity = header.Salinity,
                SalinityType = ShearwaterUtilsWrapper.GetSalinityType(shearwaterDiveLog),

                // Computer Info
                ComputerModel = ShearwaterUtilsWrapper.GetComputerName(shearwaterDiveLog),
                ComputerSerialNumber = ShearwaterUtilsWrapper.GetComputerSerialNumber(shearwaterDiveLog),
                ComputerFirmwareVersion = (int)header.FirmwareVersion,
                BatteryType = ShearwaterUtilsWrapper.GetComputerBatteryType(shearwaterDiveLog),
                BatteryVoltagePreDive = header.InternalBatteryVoltage,
                BatteryVoltagePostDive = footer.InternalBatteryVoltage,
                SampleRateInMs = header.SampleRateMs,
                DataFormat = $"{interpretedLog.DiveLogDataFormat}-{ShearwaterUtilsWrapper.GetDiveLogVersion(shearwaterDiveLog)}-{shearwaterDiveLog.DbVersion}",
            };

            if (!ShearwaterUtilsWrapper.IsFreeDive(shearwaterDiveLog))
            {
                // Optional Deco Info
                Summary.DecoModel = ShearwaterUtilsWrapper.GetDecoModel(shearwaterDiveLog);
                Summary.GradientFactorLow = header.GradientFactorLow;
                Summary.GradientFactorHigh = header.GradientFactorHigh;
                Summary.GradientFactor99Max = interpretedLog.PeakEndGF99;
                Summary.CNSPercentPreDive = header.CnsPercent;
                Summary.CNSPercentPostDive = footer.CnsPercent;
            }

            if (!ShearwaterUtilsWrapper.IsFreeDive(shearwaterDiveLog))
            {
                Tank = new ExportedDiveLogTank
                {
                    Number = number,
                };

                (Tank.Tank1Enabled, Tank.Tank1TransmitterName, Tank.Tank1TransmitterSerialNumber, Tank.Tank1AverageDepthInMeters, Tank.Tank1GasO2Percent, Tank.Tank1GasHePercent, Tank.Tank1GasN2Percent) = ShearwaterUtilsWrapper.GetTankInfo(shearwaterDiveLog, 0);
                (Tank.Tank2Enabled, Tank.Tank2TransmitterName, Tank.Tank2TransmitterSerialNumber, Tank.Tank2AverageDepthInMeters, Tank.Tank2GasO2Percent, Tank.Tank2GasHePercent, Tank.Tank2GasN2Percent) = ShearwaterUtilsWrapper.GetTankInfo(shearwaterDiveLog, 1);
                (Tank.Tank3Enabled, Tank.Tank3TransmitterName, Tank.Tank3TransmitterSerialNumber, Tank.Tank3AverageDepthInMeters, Tank.Tank3GasO2Percent, Tank.Tank3GasHePercent, Tank.Tank3GasN2Percent) = ShearwaterUtilsWrapper.GetTankInfo(shearwaterDiveLog, 2);
                (Tank.Tank4Enabled, Tank.Tank4TransmitterName, Tank.Tank4TransmitterSerialNumber, Tank.Tank4AverageDepthInMeters, Tank.Tank4GasO2Percent, Tank.Tank4GasHePercent, Tank.Tank4GasN2Percent) = ShearwaterUtilsWrapper.GetTankInfo(shearwaterDiveLog, 3);
            }
            Samples = new List<ExportedDiveLogSample>();

            foreach (var shearwaterDiveLogSample in shearwaterDiveLogSamples)
            {
                if (shearwaterDiveLogSample.RawBytes != null)
                {
                    continue;
                }

                var sample = new ExportedDiveLogSample
                {
                    Number = number,
                    ElapsedTimeInSeconds = (int)shearwaterDiveLogSample.TimeSinceStartInSeconds,
                    Depth = ShearwaterUtilsWrapper.GetDepthInMeters(shearwaterDiveLog, shearwaterDiveLogSample),
                    Temperature = shearwaterDiveLogSample.WaterTemperature,
                    BatteryVoltage = shearwaterDiveLogSample.BatteryVoltage,
                };

                if (!ShearwaterUtilsWrapper.IsFreeDive(shearwaterDiveLog))
                {

                    sample.TimeToSurfaceInMinutes = shearwaterDiveLogSample.TimeToSurface;
                    sample.TimeToSurfaceInMinutesAtPlusFive = shearwaterDiveLogSample.AtPlusFive;
                    sample.NoDecoLimit = shearwaterDiveLogSample.CurrentNoDecoLimit;
                    sample.CNS = shearwaterDiveLogSample.CentralNervousSystemPercentage;
                    sample.GasDensity = ShearwaterUtilsWrapper.GetGasDensityInGPerL(shearwaterDiveLog, shearwaterDiveLogSample);
                    sample.GradientFactor99 = shearwaterDiveLogSample.Gf99;
                    sample.PPO2 = shearwaterDiveLogSample.AveragePPO2;
                    (sample.PPN2, sample.PPHe) = ShearwaterUtilsWrapper.GetGasPartialPressureInAta(shearwaterDiveLog, shearwaterDiveLogSample);
                    sample.Tank1PressureInBar = ShearwaterUtilsWrapper.GetTankPressureInBar(shearwaterDiveLogSample, 0);
                    sample.Tank2PressureInBar = ShearwaterUtilsWrapper.GetTankPressureInBar(shearwaterDiveLogSample, 1);
                    sample.Tank3PressureInBar = ShearwaterUtilsWrapper.GetTankPressureInBar(shearwaterDiveLogSample, 2);
                    sample.Tank4PressureInBar = ShearwaterUtilsWrapper.GetTankPressureInBar(shearwaterDiveLogSample, 3);
                    sample.SurfaceAirConsumptionInBar = ShearwaterUtilsWrapper.GetSurfaceAirConsumptionInBar(shearwaterDiveLogSample);
                    sample.GasTimeRemainingInMinutes = ShearwaterUtilsWrapper.GetGasTimeRemainingInMinutes(shearwaterDiveLogSample);
                }

                Samples.Add(sample);
            }
        }
    }
}

using System;
using System.Collections.Generic;
using Assets.DeepLinkingForWindows;
using Assets.Scripts.DiveLogs.Utils.DiveLogUtils;
using Assets.Scripts.DiveLogs.Utils.Gases;
using Assets.Scripts.Utility;
using Assets.ShearwaterCloud.Modules.Graphs.DiveGraph.GraphAssembly.GraphDataAssembly.SeriesSampleAssemblers;
using CoreParserUtilities;
using DiveLogModels;
using ExtendedCoreParserUtilities;
using Shearwater;
using ShearwaterUtils;

namespace DiveLogExporter
{
    internal class ExportedDiveLog
    {
        public ExportedDiveLogSummary Summary { get; set; }

        public ExportedDiveLogTank Tank { get; set; }

        public List<ExportedDiveLogSample> Samples { get; set; }

        public ExportedDiveLog(DiveLog shearwaterDiveLog, List<DiveLogSample> shearwaterDiveLogSamples)
        {
            var header = shearwaterDiveLog.DiveLogHeader;
            var footer = shearwaterDiveLog.DiveLogFooter;
            var details = shearwaterDiveLog.DiveLogDetails;
            var interpretedLog = shearwaterDiveLog.InterpretedLogData;
            var finalLog = shearwaterDiveLog.FinalLog;
            var tankProfileData = TankProfileSerializer.ConvertStringToTankProfileData(shearwaterDiveLog.DiveLogDetails.TankProfileData.Value);
            Summary = new ExportedDiveLogSummary
            {
                // Summary Info
                Number = int.Parse(DiveLogMetaDataResolver.GetDiveNumber(shearwaterDiveLog)),
                Mode = DiveLogModeUtils.GetModeName(header.Mode, header.OCRecSubMode, DiveLogMetaDataResolver.GetLogVersion(shearwaterDiveLog)),
                StartDate = shearwaterDiveLog.DiveLogDetails.DiveDate.ToString(),
                EndDate = (shearwaterDiveLog.DiveLogDetails.DiveDate + (default(DateTime).FromUnixTimeStamp(footer.Timestamp) - default(DateTime).FromUnixTimeStamp(header.Timestamp))).ToString(),
                DurationInSeconds = footer.DiveTimeInSeconds,
                DepthInMetersMax = footer.MaxDiveDepth,
                DepthInMetersAvg = interpretedLog.AverageDepth,
                Buddy = shearwaterDiveLog.DiveLogDetails.Buddy.Value,
                Location = shearwaterDiveLog.DiveLogDetails.Location.Value,
                Site = shearwaterDiveLog.DiveLogDetails.Site.Value,
                Note = shearwaterDiveLog.DiveLogDetails.Notes.Value,

                // Environment Info
                TemperatureInCelsiusMax = interpretedLog.MaxTemp,
                TemperatureInCelsiusMin = interpretedLog.MinTemp,
                TemperatureInCelsiusAvg = interpretedLog.AverageTemp,
                Salinity = DiveLogEnvironmentUtils.GetSalinityString(header.Salinity),
                SurfaceIntervalInSeconds = (int)TimeSpan.FromMinutes(header.SurfaceTime).TotalSeconds,
                SurfacePressureInMillibarPreDive = header.SurfacePressure,
                SurfacePressureInMillibarPostDive = footer.SurfacePressure,

                // Deco Info
                DecoModel = DecoModelUtil.DecoModelString(header.DecoModel),
                GradientFactorLow = header.GradientFactorLow,
                GradientFactorHigh = header.GradientFactorHigh,
                GradientFactor99Max = interpretedLog.PeakEndGF99,
                CNSPercentPreDive = header.CnsPercent,
                CNSPercentPostDive = footer.CnsPercent,

                // Computer Info
                ComputerModel = DiveLogProductUtilMod.FriendlyProductName(finalLog),
                ComputerSerialNumber = DiveLogSerialNumberUtil.GetSerialNumberToHex(shearwaterDiveLog),
                ComputerFirmwareVersion = (int)header.FirmwareVersion,
                BatteryType = DiveLogBatteryUtil.GetBatteryType(header),
                BatteryVoltagePreDive = header.InternalBatteryVoltage,
                BatteryVoltagePostDive = footer.InternalBatteryVoltage,
                SampleRateInMs = header.SampleRateMs,
                DataFormat = interpretedLog.DiveLogDataFormat,
                LogVersion = DiveLogMetaDataResolver.GetLogVersion(shearwaterDiveLog),
                DatabaseVersion = (int)shearwaterDiveLog.DbVersion,

                // Others
                O2SensorStatusPreDive = header.O2Sensor1Status,
                O2SensorStatusPostDive = footer.O2Sensor1Status,
                SensorDisplay = header.SensorDisplay,
                PPO2SetpointLowPreDive = header.LowPPO2Setpoint,
                PPO2SetpointLowPostDive = footer.LowPPO2Setpoint,
                PPO2SetpointHighPreDive = header.HighPPO2Setpoint,
                PPO2SetpointHighPostDive = footer.HighPPO2Setpoint,
                Features = finalLog.Features,
            };

            Tank = new ExportedDiveLogTank
            {
                Number = Summary.Number,

                Tank1Enabled = tankProfileData.TankData[0].DiveTransmitter.IsOn,
                Tank1TransmitterName = tankProfileData.TankData[0].DiveTransmitter.Name,
                Tank1TransmitterSerialNumber = DiveLogSerialNumberUtil.FormatAiSerialNumber(shearwaterDiveLog, tankProfileData.TankData[0].DiveTransmitter.UnformattedSerialNumber),
                Tank1AverageDepthInMeters = tankProfileData.TankData[0].GasProfile.AverageDepthInMeters,
                Tank1GasO2Percent = tankProfileData.TankData[0].GasProfile.O2Percent,
                Tank1GasHePercent = tankProfileData.TankData[0].GasProfile.HePercent,
                Tank1GasN2Percent = 100 - tankProfileData.TankData[0].GasProfile.O2Percent - tankProfileData.TankData[0].GasProfile.HePercent,

                Tank2Enabled = tankProfileData.TankData[1].DiveTransmitter.IsOn,
                Tank2TransmitterName = tankProfileData.TankData[1].DiveTransmitter.Name,
                Tank2TransmitterSerialNumber = DiveLogSerialNumberUtil.FormatAiSerialNumber(shearwaterDiveLog, tankProfileData.TankData[1].DiveTransmitter.UnformattedSerialNumber),
                Tank2AverageDepthInMeters = tankProfileData.TankData[1].GasProfile.AverageDepthInMeters,
                Tank2GasO2Percent = tankProfileData.TankData[1].GasProfile.O2Percent,
                Tank2GasHePercent = tankProfileData.TankData[1].GasProfile.HePercent,
                Tank2GasN2Percent = 100 - tankProfileData.TankData[1].GasProfile.O2Percent - tankProfileData.TankData[1].GasProfile.HePercent,

                Tank3Enabled = tankProfileData.TankData[2].DiveTransmitter.IsOn,
                Tank3TransmitterName = tankProfileData.TankData[2].DiveTransmitter.Name,
                Tank3TransmitterSerialNumber = DiveLogSerialNumberUtil.FormatAiSerialNumber(shearwaterDiveLog, tankProfileData.TankData[2].DiveTransmitter.UnformattedSerialNumber),
                Tank3AverageDepthInMeters = tankProfileData.TankData[2].GasProfile.AverageDepthInMeters,
                Tank3GasO2Percent = tankProfileData.TankData[2].GasProfile.O2Percent,
                Tank3GasHePercent = tankProfileData.TankData[2].GasProfile.HePercent,
                Tank3GasN2Percent = 100 - tankProfileData.TankData[2].GasProfile.O2Percent - tankProfileData.TankData[2].GasProfile.HePercent,

                Tank4Enabled = tankProfileData.TankData[3].DiveTransmitter.IsOn,
                Tank4TransmitterName = tankProfileData.TankData[3].DiveTransmitter.Name,
                Tank4TransmitterSerialNumber = DiveLogSerialNumberUtil.FormatAiSerialNumber(shearwaterDiveLog, tankProfileData.TankData[3].DiveTransmitter.UnformattedSerialNumber),
                Tank4AverageDepthInMeters = tankProfileData.TankData[3].GasProfile.AverageDepthInMeters,
                Tank4GasO2Percent = tankProfileData.TankData[3].GasProfile.O2Percent,
                Tank4GasHePercent = tankProfileData.TankData[3].GasProfile.HePercent,
                Tank4GasN2Percent = 100 - tankProfileData.TankData[3].GasProfile.O2Percent - tankProfileData.TankData[3].GasProfile.HePercent,
            };

            Samples = new List<ExportedDiveLogSample>();

            foreach (var shearwaterDiveLogSample in shearwaterDiveLogSamples)
            {
                if (shearwaterDiveLogSample.RawBytes != null)
                {
                    continue;
                }

                var absolutePressureInAta = GasUtil.GetAbsolutePressureATA((float)header.SurfacePressure, ConvertDepthToMeters(header, shearwaterDiveLogSample.Depth), false);
                var partialPressures = GasUtil.FindInertGasPartialPressures(shearwaterDiveLogSample.AveragePPO2, absolutePressureInAta, shearwaterDiveLogSample.FractionO2, shearwaterDiveLogSample.FractionHe);
                var tank1Message = DiveLogGasMessageRetrieverMod.Get_Tank0_Message(shearwaterDiveLogSample);
                var tank2Message = DiveLogGasMessageRetrieverMod.Get_Tank1_Message(shearwaterDiveLogSample);
                var tank3Message = DiveLogGasMessageRetrieverMod.Get_Tank2_Message(shearwaterDiveLogSample);
                var tank4Message = DiveLogGasMessageRetrieverMod.Get_Tank3_Message(shearwaterDiveLogSample);
                var sacMessage = DiveLogGasMessageRetrieverMod.Get_SAC_Message(shearwaterDiveLogSample);

                Samples.Add(new ExportedDiveLogSample
                {
                    Number = Summary.Number,
                    ElapsedTimeInSeconds = (int)shearwaterDiveLogSample.TimeSinceStartInSeconds,
                    Depth = ConvertDepthToMeters(header, shearwaterDiveLogSample.Depth),
                    TimeToSurfaceInMinutes = shearwaterDiveLogSample.TimeToSurface,
                    TimeToSurfaceInMinutesAtPlusFive = shearwaterDiveLogSample.AtPlusFive,
                    NoDecoLimit = shearwaterDiveLogSample.CurrentNoDecoLimit,
                    CNS = shearwaterDiveLogSample.CentralNervousSystemPercentage,
                    GasDensity = GraphSampleGasDensity.GasDensityFormulaOpenCircuit(shearwaterDiveLogSample, absolutePressureInAta),
                    GradientFactor99 = shearwaterDiveLogSample.Gf99,
                    PPO2 = shearwaterDiveLogSample.AveragePPO2,
                    PPN2 = partialPressures.ppN2ATA,
                    PPHE = partialPressures.ppHeATA,
                    Tank1PressureInBar = int.TryParse(tank1Message, out int tank1PressureInPSI) ? UnitConverter.ConvertTankUnits(tank1PressureInPSI, SettingDefinitions.TankUnits.Bar) : 0,
                    Tank2PressureInBar = int.TryParse(tank2Message, out int tank2PressureInPSI) ? UnitConverter.ConvertTankUnits(tank2PressureInPSI, SettingDefinitions.TankUnits.Bar) : 0,
                    Tank3PressureInBar = int.TryParse(tank3Message, out int tank3PressureInPSI) ? UnitConverter.ConvertTankUnits(tank3PressureInPSI, SettingDefinitions.TankUnits.Bar) : 0,
                    Tank4PressureInBar = int.TryParse(tank4Message, out int tank4PressureInPSI) ? UnitConverter.ConvertTankUnits(tank4PressureInPSI, SettingDefinitions.TankUnits.Bar) : 0,
                    SAC = int.TryParse(sacMessage, out int sacInPSI) ? UnitConverter.ConvertTankUnits(sacInPSI, SettingDefinitions.TankUnits.Bar) : 0,
                    Temperature = shearwaterDiveLogSample.WaterTemperature,
                    BatteryVoltage = shearwaterDiveLogSample.BatteryVoltage,
                    GasTimeRemainingInMinutes = int.TryParse(DiveLogGasMessageRetrieverMod.Get_GasTime_Message(shearwaterDiveLogSample), out int gasTimeRemaining) ? gasTimeRemaining : 0
                });
            }
        }
        private static float ConvertDepthToMeters(DiveLogHeader diveLogHeader, float depth)
        {
            if (depth > 1000)
            {
                return UnitConverter.Convert_pressure_mBars_to_depth_m_f(depth, diveLogHeader.SurfacePressure, diveLogHeader.Salinity);
            }

            return depth;
        }
    }
}

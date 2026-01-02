using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using Assets.Scripts.DiveLogs.Utils.DiveLogSampleUtils;
using Assets.Scripts.FileFormats.Export;
using Assets.Scripts.FileFormats.Legacy.ShearwaterXML;
using CoreParserUtilities;
using DiveLogModels;
using ExtendedCoreParserUtilities;
using ShearwaterUtils;

namespace Shearwater
{
    public class ShearwaterXMLExporterMod : IDiveExport
    {
        public ExportDataType[] ExportDataTypes => new ExportDataType[2]
        {
        ExportDataType.DiveLog,
        ExportDataType.Samples
        };

        public void StartExportSession(string[] diveIds, string path)
        {
        }

        public void EndExportSession()
        {
        }

        public Task<string> ExportDive(object[] exportData, string filename)
        {
            DiveLog diveLog = exportData[0] as DiveLog;
            DiveLogSample[] array = exportData[1] as DiveLogSample[];
            if (diveLog == null || array == null)
            {
                return Task.FromResult("INVALID_EXPORT_DATA");
            }

            dive o = ToShearwaterXML(diveLog, array);
            XmlSerializer xmlSerializer = new XmlSerializer(typeof(dive));
            StringBuilder stringBuilder = new StringBuilder();
            StringWriter textWriter = new StringWriter(stringBuilder);
            xmlSerializer.Serialize(textWriter, o);
            XmlDocument xmlDocument = new XmlDocument();
            xmlDocument.LoadXml(stringBuilder.ToString());
            if (diveLog.DiveLogHeader.DecoModel == 3)
            {
                XmlNode xmlNode = xmlDocument.SelectSingleNode("dive/diveLog/gfMin");
                XmlNode xmlNode2 = xmlDocument.SelectSingleNode("dive/diveLog/gfMax");
                xmlNode?.ParentNode?.RemoveChild(xmlNode);
                xmlNode2?.ParentNode?.RemoveChild(xmlNode2);
            }

            StringWriter stringWriter = new StringWriter();
            XmlTextWriter xmlTextWriter = new XmlTextWriter(stringWriter)
            {
                Formatting = Formatting.Indented
            };
            xmlDocument.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            File.WriteAllText(filename, stringWriter.ToString());
            return Task.FromResult(string.Empty);
        }

        private static dive ToShearwaterXML(DiveLog diveLog, DiveLogSample[] diveLogSamples)
        {
            DiveLogHeader diveLogHeader = diveLog.DiveLogHeader;
            DiveLogFooter diveLogFooter = diveLog.DiveLogFooter;
            FinalLog finalLog = diveLog.FinalLog;
            TimeSpan timeSpan = default(DateTime).FromUnixTimeStamp(diveLog.DiveLogFooter.Timestamp) - default(DateTime).FromUnixTimeStamp(diveLog.DiveLogHeader.Timestamp);
            return new dive
            {
                version = 3,
                diveLog = new diveDiveLog
                {
                    number = DiveLogMetaDataResolver.GetDiveNumber(diveLog),
                    gfMin = diveLogHeader.GradientFactorLow,
                    gfMax = diveLogHeader.GradientFactorHigh,
                    surfaceMins = diveLogHeader.SurfaceTime,
                    imperialUnits = !DiveLogDepthUtil.DepthUnitsAreMetric(diveLog),
                    startBatteryVoltage = diveLogHeader.InternalBatteryVoltage,
                    startCns = diveLogHeader.CnsPercent,
                    startO2SensorStatus = diveLogHeader.O2Sensor1Status,
                    logVersion = DiveLogMetaDataResolver.GetLogVersion(diveLog),
                    decoModel = diveLogHeader.DecoModel,
                    computerFirmware = diveLogHeader.FirmwareVersion.ToString("X"),
                    sensorDisplay = diveLogHeader.SensorDisplay,
                    startSurfacePressure = diveLogHeader.SurfacePressure,
                    startLowSetPoint = diveLogHeader.LowPPO2Setpoint,
                    startHighSetPoint = diveLogHeader.HighPPO2Setpoint,
                    vpmbConservatism = diveLogHeader.VpmbConservatism,
                    startDate = diveLog.DiveLogDetails.DiveDate.ToString(),
                    endBatteryVoltage = diveLogFooter.InternalBatteryVoltage,
                    endCns = diveLogFooter.CnsPercent,
                    endDate = (diveLog.DiveLogDetails.DiveDate + timeSpan).ToString(),
                    endHighSetPoint = diveLogFooter.HighPPO2Setpoint,
                    endLowSetPoint = diveLogFooter.LowPPO2Setpoint,
                    endO2SensorStatus = diveLogFooter.O2Sensor1Status,
                    endSurfacePressure = diveLogFooter.SurfacePressure,
                    errorHistory = diveLogFooter.ErrorFlags0,
                    maxDepth = diveLogFooter.MaxDiveDepth,
                    maxTime = diveLogFooter.DiveTimeInSeconds,
                    computerSerial = DiveLogSerialNumberUtil.GetSerialNumber(diveLog).ToString("X"),
                    computerSoftwareVersion = diveLogHeader.FirmwareVersion.ToString("X"),
                    product = finalLog.Product,
                    computerModel = finalLog.ComputerModel,
                    features = finalLog.Features,
                    diveLogRecords = getRecordsFromDiveLog(diveLog, diveLogSamples)
                },
                versionSpecified = true
            };


        }

        private static diveDiveLogDiveLogRecord[] getRecordsFromDiveLog(DiveLog diveLog, DiveLogSample[] diveLogSamples)
        {
            DiveLogHeader diveLogHeader = diveLog.DiveLogHeader;
            DiveLogFooter diveLogFooter = diveLog.DiveLogFooter;
            bool isFreeDive = DiveLogModeUtils.GetModeName(diveLogHeader.Mode, diveLogHeader.OCRecSubMode, DiveLogMetaDataResolver.GetLogVersion(diveLog)) == DiveLogModeUtils.MODE_NAME_FREEDIVE;

            List<diveDiveLogDiveLogRecord> list = new List<diveDiveLogDiveLogRecord>();
            foreach (DiveLogSample diveLogSample in diveLogSamples)
            {
                if (diveLogSample.RawBytes == null)
                {
                    diveDiveLogDiveLogRecord item = new diveDiveLogDiveLogRecord
                    {
                        currentTime = (int)diveLogSample.TimeSinceStartInSeconds,
                        gasTime = DiveLogGasMessageRetrieverMod.Get_GasTime_Message(diveLogSample),
                        sensor1Millivolts = diveLogSample.Sensor1Millivolts,
                        currentCircuitSetting = DiveLogModeUtils.GetCircuitModeName(DiveLogModeUtils.GetDiveModeValueFromCircuitSetting(diveLogSample.CircuitMode, diveLogSample.CircuitSwitchType)),
                        currentCcrModeSettings = diveLogSample.CcrMode,
                        externalPPO2 = diveLogSample.ExternalPPO2,
                        fractionHe = diveLogSample.FractionHe,
                        batteryVoltage = diveLogSample.BatteryVoltage,
                        firstStopTime = diveLogSample.NextStopTime,
                        sensor3Millivolts = diveLogSample.Sensor3Millivolts,
                        currentDepth = ConvertDepthToMeters(diveLogHeader, diveLogSample.Depth),
                        circuitSwitchType = diveLogSample.CircuitSwitchType,
                        gasSwitchNeeded = diveLogSample.GasSwitchNeeded,
                        averagePPO2 = diveLogSample.AveragePPO2,
                        firstStopDepth = diveLogSample.NextStopDepth,
                        setPointType = diveLogSample.SetPointType,
                        ttsMins = diveLogSample.TimeToSurface,
                        waterTemp = diveLogSample.WaterTemperature,
                        currentNdl = diveLogSample.CurrentNoDecoLimit,
                        fractionO2 = diveLogSample.FractionO2,
                        sac = DiveLogGasMessageRetrieverMod.Get_SAC_Message(diveLogSample),
                        sensor2Millivolts = diveLogSample.Sensor2Millivolts,
                        tank0pressurePSI = DiveLogGasMessageRetrieverMod.Get_Tank0_Message(diveLogSample),
                        tank1pressurePSI = DiveLogGasMessageRetrieverMod.Get_Tank1_Message(diveLogSample),
                        tank2pressurePSI = DiveLogGasMessageRetrieverMod.Get_Tank2_Message(diveLogSample),
                        tank3pressurePSI = DiveLogGasMessageRetrieverMod.Get_Tank3_Message(diveLogSample),
                        sad = diveLogSample.SafeAscentDepth
                    };
                    list.Add(item);
                }
            }
            return list.ToArray();
        }

        private static float ConvertDepthToMeters(DiveLogHeader diveLogHeader, float depth)
        {
            if (diveLogHeader.DepthUnitSystem == 0)
            {
                return UnitConverter.Convert_pressure_mBars_to_depth_m_f(depth, diveLogHeader.SurfacePressure, diveLogHeader.Salinity);
            }

            return depth;
        }
    }
}

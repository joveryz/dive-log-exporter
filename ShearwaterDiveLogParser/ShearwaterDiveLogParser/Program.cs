using Assets.Scripts.DiveLogs.Utils.DiveLogSampleUtils;
using Assets.Scripts.DiveLogs.Utils.Gases;
using Assets.Scripts.Extensions;
using Assets.Scripts.FileFormats.Export;
using Assets.Scripts.FileFormats.Import.DiveLogParser.Utility;
using Assets.Scripts.FileFormats.Legacy.ShearwaterXML;
using Assets.Scripts.Persistence.LocalCache;
using Assets.Scripts.Persistence.LocalCache.Schema;
using CoreParserUtilities;
using DiveLogModels;
using DiveLogParser;
using ExtendedCoreParserUtilities;
using I2.Loc;
using Newtonsoft.Json;
using ShearwaterUtils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using ShearwaterDiveLogParser.Shearwater;

namespace ShearwaterDiveLogParser
{
    internal class Program
    {
        static void Main(string[] args)
        {
            // Parse whole shearwater source file
            //byte[] bin = File.ReadAllBytes("D:\\sf_98468741764873424_Teric[E096404C]#153 2025-12-4 18-37-4.swlogdata");
            //IDiveLogParser parser = RawDiveLogParserDecider.Decide(bin);
            //DiveLog log = parser.GetDiveLog(bin);

            DataService dataService = new DataService("D:\\UserData\\Desktop\\jovery@zmailing.com_2025-12-26.db");
            LocalCache localCache = new LocalCache(dataService);
            string[] allIds = dataService.GetAllIds();
            //var (logs, report) = dataService.GetDiveLogsAsync(allIds).Result;
            var logs = dataService.GetDiveLogsWithRaw();

            var id = allIds[23];
            var log = dataService.GetDiveLog(id);
            var samples = dataService.GetDiveLogRecordsWithRaw(id);

            object[] para = new object[2];
            para[0] = log;
            para[1] = samples.ToArray();
            DiveLog divelog = para[0] as DiveLog;
            DiveLogSample[] sampleRecords = para[1] as DiveLogSample[];
            ShearwaterXMLExporterMod exporter = new ShearwaterXMLExporterMod();
            exporter.ExportDive(para, "D:\\123333");


            // Parse log_data.data_bytes_1 and log_data.data_bytes_2 blobs from shearwater exported sqlite db with type sw-clouddb
            //byte[] bin1 = File.ReadAllBytes("<Path to data_bytes_1.bin>").FromCompressedByteArray();
            //dive_logs info = JsonConvert.DeserializeObject<dive_logs>(Encoding.Default.GetString(bin1));

            //byte[] bin2 = File.ReadAllBytes("<Path to data_bytes_2.bin>").FromCompressedByteArray();
            //dive_log_records[] samples = JsonConvert.DeserializeObject<dive_log_records[]>(Encoding.Default.GetString(bin2));

            // Parse log_data.data_bytes_1 blob from shearwater exported sqlite db with type sw-pnf
            //byte[] bin3 = File.ReadAllBytes("<Path to data_bytes_1.bin>").FromCompressedByteArray();
            //RawDiveLogType type3 = RawDiveLogDescriminator.WhatKindIsThis(bin3);
            //DiveLog rawLog3 = RawDiveLogParserDecider.Decide(bin).GetDiveLog(bin3);
        }

        private static void InternalMethodWrapper(object[] para)
        {
            var a = ExportDive(para, "D:\\123333");
        }

        public static string ExportDive(object[] exportData, string filename)
        {
            DiveLog diveLog = exportData[0] as DiveLog;
            DiveLogSample[] array = exportData[1] as DiveLogSample[];
            if (diveLog == null || array == null)
            {
                return "INVALID_EXPORT_DATA";
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
                Formatting = System.Xml.Formatting.Indented
            };
            xmlDocument.WriteTo(xmlTextWriter);
            xmlTextWriter.Flush();
            File.WriteAllText(filename, stringWriter.ToString());
            return String.Empty;
        }

        private static dive ToShearwaterXML(DiveLog divelog, DiveLogSample[] sampleRecords)
        {
            DiveLogHeader diveLogHeader = divelog.DiveLogHeader;
            DiveLogFooter diveLogFooter = divelog.DiveLogFooter;
            FinalLog finalLog = divelog.FinalLog;
            TimeSpan timeSpan = new DateTime().FromUnixTimeStamp(divelog.DiveLogFooter.Timestamp) - new DateTime().FromUnixTimeStamp(divelog.DiveLogHeader.Timestamp);
            return new dive()
            {
                version = 3,
                diveLog = new diveDiveLog()
                {
                    number = DiveLogMetaDataResolver.GetDiveNumber(divelog),
                    gfMin = diveLogHeader.GradientFactorLow,
                    gfMax = diveLogHeader.GradientFactorHigh,
                    surfaceMins = diveLogHeader.SurfaceTime,
                    imperialUnits = !DiveLogDepthUtil.DepthUnitsAreMetric(divelog),
                    startBatteryVoltage = diveLogHeader.InternalBatteryVoltage,
                    startCns = diveLogHeader.CnsPercent,
                    startO2SensorStatus = diveLogHeader.O2Sensor1Status,
                    logVersion = DiveLogMetaDataResolver.GetLogVersion(divelog),
                    decoModel = diveLogHeader.DecoModel,
                    computerFirmware = diveLogHeader.FirmwareVersion.ToString("X"),
                    sensorDisplay = diveLogHeader.SensorDisplay,
                    startSurfacePressure = diveLogHeader.SurfacePressure,
                    startLowSetPoint = diveLogHeader.LowPPO2Setpoint,
                    startHighSetPoint = diveLogHeader.HighPPO2Setpoint,
                    vpmbConservatism = diveLogHeader.VpmbConservatism,
                    startDate = divelog.DiveLogDetails.DiveDate.ToString(),
                    endBatteryVoltage = diveLogFooter.InternalBatteryVoltage,
                    endCns = diveLogFooter.CnsPercent,
                    endDate = (divelog.DiveLogDetails.DiveDate + timeSpan).ToString(),
                    endHighSetPoint = diveLogFooter.HighPPO2Setpoint,
                    endLowSetPoint = diveLogFooter.LowPPO2Setpoint,
                    endO2SensorStatus = diveLogFooter.O2Sensor1Status,
                    endSurfacePressure = diveLogFooter.SurfacePressure,
                    errorHistory = diveLogFooter.ErrorFlags0,
                    maxDepth = diveLogFooter.MaxDiveDepth,
                    maxTime = diveLogFooter.DiveTimeInSeconds,
                    computerSerial = DiveLogSerialNumberUtil.GetSerialNumber(divelog).ToString("X"),
                    computerSoftwareVersion = diveLogHeader.FirmwareVersion.ToString("X"),
                    product = finalLog.Product,
                    computerModel = finalLog.ComputerModel,
                    features = finalLog.Features,
                    diveLogRecords = getRecordsFromDiveLog(sampleRecords, diveLogHeader.SampleRateMs)
                },
                versionSpecified = true
            };
        }


        private static diveDiveLogDiveLogRecord[] getRecordsFromDiveLog(DiveLogSample[] sampleRecords, int sampleRateMs)
        {
            List<diveDiveLogDiveLogRecord> logDiveLogRecordList = new List<diveDiveLogDiveLogRecord>();
            foreach (DiveLogSample sampleRecord in sampleRecords)
            {
                if (sampleRecord.RawBytes == null)
                {
                    diveDiveLogDiveLogRecord logDiveLogRecord = new diveDiveLogDiveLogRecord();
                    logDiveLogRecord.currentTime = (int)UnitConverter.SecondsToMs(sampleRecord.TimeSinceStartInSeconds);
                    logDiveLogRecord.gasTime = DiveLogGasMessageRetriever.Get_GasTime_Message(sampleRecord);
                    logDiveLogRecord.sensor1Millivolts = sampleRecord.Sensor1Millivolts;
                    logDiveLogRecord.currentCircuitSetting = GetCircuitModeName(GetDiveModeValueFromCircuitSetting(sampleRecord.CircuitMode, sampleRecord.CircuitSwitchType));
                    logDiveLogRecord.currentCcrModeSettings = sampleRecord.CcrMode;
                    logDiveLogRecord.externalPPO2 = sampleRecord.ExternalPPO2;
                    logDiveLogRecord.fractionHe = sampleRecord.FractionHe;
                    logDiveLogRecord.batteryVoltage = sampleRecord.BatteryVoltage;
                    logDiveLogRecord.firstStopTime = sampleRecord.NextStopTime;
                    logDiveLogRecord.sensor3Millivolts = sampleRecord.Sensor3Millivolts;
                    logDiveLogRecord.currentDepth = sampleRecord.Depth;
                    logDiveLogRecord.circuitSwitchType = sampleRecord.CircuitSwitchType;
                    logDiveLogRecord.gasSwitchNeeded = sampleRecord.GasSwitchNeeded;
                    logDiveLogRecord.averagePPO2 = sampleRecord.AveragePPO2;
                    logDiveLogRecord.firstStopDepth = sampleRecord.NextStopDepth;
                    logDiveLogRecord.setPointType = sampleRecord.SetPointType;
                    logDiveLogRecord.ttsMins = sampleRecord.TimeToSurface;
                    logDiveLogRecord.waterTemp = sampleRecord.WaterTemperature;
                    logDiveLogRecord.currentNdl = sampleRecord.CurrentNoDecoLimit;
                    logDiveLogRecord.fractionO2 = sampleRecord.FractionO2;
                    logDiveLogRecord.sac = Get_SAC_Message(sampleRecord);
                    logDiveLogRecord.sensor2Millivolts = sampleRecord.Sensor2Millivolts;
                    //logDiveLogRecord.tank0pressurePSI = DiveLogGasMessageRetriever.Get_Tank0_Message(sampleRecord);
                    //logDiveLogRecord.tank1pressurePSI = DiveLogGasMessageRetriever.Get_Tank1_Message(sampleRecord);
                    //logDiveLogRecord.tank2pressurePSI = DiveLogGasMessageRetriever.Get_Tank2_Message(sampleRecord);
                    //logDiveLogRecord.tank3pressurePSI = DiveLogGasMessageRetriever.Get_Tank3_Message(sampleRecord);
                    logDiveLogRecord.sad = sampleRecord.SafeAscentDepth;
                    logDiveLogRecordList.Add(logDiveLogRecord);
                }
            }
            return logDiveLogRecordList.ToArray();
        }

        public static string GetCircuitModeName(int RecordMode)
        {
            return RecordMode switch
            {
                1 => "CC/BO",
                0 => "OC/BO",
                2 => "SC/BO",
                _ => "Unknown",
            };
        }

        public static string Get_SAC_Message(DiveLogSample sample, bool applyUnitPref = false)
        {
            return GetSacData(sample, ParseSacMessage_verbose((int)(sample.Sac * 100f)), applyUnitPref);
        }

        public static int GetDiveModeValueFromCircuitSetting(int circuitMode, int circuitSwitchType)
        {
            if (circuitMode == 1)
            {
                return 0;
            }

            if (circuitSwitchType == 1)
            {
                return 2;
            }

            return 1;
        }

        public static string GetSacData(DiveLogSample sample, string sacMessage, bool applyUnitPref = false)
        {
            string text = ParseSacMessage_verbose(sample.sac_data);
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (!string.IsNullOrEmpty(sacMessage))
            {
                return sacMessage;
            }

            if (applyUnitPref)
            {
                return UnitConverter.ConvertTankUnits(sample.Sac, Settings.TankUnit).ToString();
            }

            return sample.Sac.ToString();
        }

        public static string ParseSacMessage_verbose(int sensorData)
        {
            switch (sensorData)
            {
                case 65535:
                    return ScriptLocalization.dive_computer.verbose_ai_is_off;
                case 65534:
                    return ScriptLocalization.dive_computer.verbose_no_comms_seconds_90;
                case 65533:
                    return ScriptLocalization.dive_computer.verbose_na_in_current_mode;
                case 65532:
                    return ScriptLocalization.dive_computer.verbose_transmitter_not_paired;
                case 65531:
                    return ScriptLocalization.dive_computer.verbose_bad_setup;
                case 65530:
                    return ScriptLocalization.dive_computer.verbose_not_diving;
                case 65529:
                    return ScriptLocalization.dive_computer.verbose_waiting_for_initial_data;
                case 65528:
                    return ScriptLocalization.dive_computer.verbose_sac_too_low;
                case 65527:
                    return ScriptLocalization.dive_computer.verbose_gtr_sac_off;
                case 65520:
                case 65521:
                case 65522:
                case 65523:
                case 65524:
                case 65525:
                case 65526:
                    return ScriptLocalization.dive_computer.not_applicable_abbreviation;
                default:
                    return "";
            }
        }
    }
}

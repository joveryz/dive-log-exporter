
using Assets.Scripts.FileFormats.Import.DiveLogParser.Utility;
using DiveLogModels;
using ShearwaterUtils;

namespace ShearwaterDiveLogParser.Shearwater
{

    public class DiveLogGasMessageRetrieverMod
    {
        public static string Get_Tank0_Message(DiveLogSample sample, bool applyUnitPref = false)
        {
            return Get_Tank0_Message_verbose(sample, applyUnitPref);
        }

        public static string Get_Tank0_Message_concise(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_concise(sample.WAISensor0Battery, sample.WAISensor0Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor0_data), sample.WAISensor0Pressure, tankMessage, applyUnitPref);
        }

        public static string Get_Tank0_Message_verbose(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_verbose(sample.WAISensor0Battery, sample.WAISensor0Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor0_data), sample.WAISensor0Pressure, tankMessage, applyUnitPref);
        }

        public static string Get_Tank1_Message(DiveLogSample sample, bool applyUnitPref = false)
        {
            return Get_Tank1_Message_verbose(sample, applyUnitPref);
        }

        public static string Get_Tank1_Message_concise(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_concise(sample.WAISensor1Battery, sample.WAISensor1Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor1_data), sample.WAISensor1Pressure, tankMessage, applyUnitPref);
        }

        public static string Get_Tank1_Message_verbose(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_verbose(sample.WAISensor1Battery, sample.WAISensor1Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor1_data), sample.WAISensor1Pressure, tankMessage, applyUnitPref);
        }

        public static string Get_Tank2_Message(DiveLogSample sample, bool applyUnitPref = false)
        {
            return Get_Tank2_Message_verbose(sample, applyUnitPref);
        }

        public static string Get_Tank2_Message_concise(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_concise(sample.WAISensor2Battery, sample.WAISensor2Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor2_data), sample.WAISensor2Pressure, tankMessage, applyUnitPref);
        }

        public static string Get_Tank2_Message_verbose(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_verbose(sample.WAISensor2Battery, sample.WAISensor2Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor2_data), sample.WAISensor2Pressure, tankMessage, applyUnitPref);
        }

        public static string Get_Tank3_Message(DiveLogSample sample, bool applyUnitPref = false)
        {
            return Get_Tank3_Message_verbose(sample, applyUnitPref);
        }

        public static string Get_Tank3_Message_concise(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_concise(sample.WAISensor3Battery, sample.WAISensor3Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor3_data), sample.WAISensor3Pressure, tankMessage, applyUnitPref);
        }

        public static string Get_Tank3_Message_verbose(DiveLogSample sample, bool applyUnitPref = false)
        {
            string tankMessage = DiveLogParserLocalizedUtil.ParseTankMessage_verbose(sample.WAISensor3Battery, sample.WAISensor3Pressure);
            return Get_Tank_Message(DiveLogParserLocalizedUtil.ParseTankMessage(sample.sensor3_data), sample.WAISensor3Pressure, tankMessage, applyUnitPref);
        }

        private static string Get_Tank_Message(string sensorData, int sensorPressure, string tankMessage, bool applyUnitPref = false)
        {
            if (!string.IsNullOrEmpty(sensorData))
            {
                return sensorData;
            }

            if (!string.IsNullOrEmpty(tankMessage))
            {
                return tankMessage;
            }

            if (applyUnitPref)
            {
                return UnitConverter.ConvertTankUnits(sensorPressure, Settings.TankUnit).ToString();
            }

            return sensorPressure.ToString();
        }

        public static string Get_GasTime_Message(DiveLogSample sample)
        {
            return Get_GasTime_Message_verbose(sample);
        }

        public static string Get_GasTime_Message_concise(DiveLogSample sample)
        {
            string gasMessage = DiveLogParserLocalizedUtil.ParseGasTimeMessage_concise(sample.WAISensorGasTimeRemaining);
            return GetGasTimeData(sample, gasMessage);
        }

        public static string Get_GasTime_Message_verbose(DiveLogSample sample)
        {
            string gasMessage = DiveLogParserLocalizedUtil.ParseGasTimeMessage_verbose(sample.WAISensorGasTimeRemaining);
            return GetGasTimeData(sample, gasMessage);
        }

        private static string GetGasTimeData(DiveLogSample sample, string gasMessage)
        {
            string text = DiveLogParserLocalizedUtil.ParseGasTimeMessage(sample.WAISensorGasTimeRemaining);
            if (!string.IsNullOrEmpty(text))
            {
                return text;
            }

            if (!string.IsNullOrEmpty(gasMessage))
            {
                return gasMessage;
            }

            return sample.WAISensorGasTimeRemaining.ToString();
        }

        public static string Get_SAC_Message(DiveLogSample sample, bool applyUnitPref = false)
        {
            return Get_SAC_Message_verbose(sample, applyUnitPref);
        }

        public static string Get_SAC_Message_concise(DiveLogSample sample, bool applyUnitPref = false)
        {
            return GetSacData(sample, DiveLogParserLocalizedUtil.ParseSacMessage_concise(sample.Sac), applyUnitPref);
        }

        public static string Get_SAC_Message_verbose(DiveLogSample sample, bool applyUnitPref = false)
        {
            return GetSacData(sample, DiveLogParserLocalizedUtil.ParseSacMessage_verbose(sample.Sac), applyUnitPref);
        }

        public static string GetSacData(DiveLogSample sample, string sacMessage, bool applyUnitPref = false)
        {
            string text = DiveLogParserLocalizedUtil.ParseSacMessage(sample.sac_data);
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
    }
}
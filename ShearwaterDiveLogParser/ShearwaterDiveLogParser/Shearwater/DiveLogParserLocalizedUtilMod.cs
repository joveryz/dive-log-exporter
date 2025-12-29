
using I2.Loc;

namespace ShearwaterDiveLogParser.Shearwater
{
    public class DiveLogParserLocalizedUtil
    {
        protected static int ReverseEngineerAISensorMessageCodeInt_legacy(int sensorBattery, int sensorPressure)
        {
            return sensorBattery + sensorPressure / 2;
        }

        protected static int ReverseEngineerSacMessageCodeInt(float sacValue)
        {
            return (int)(sacValue * 100f);
        }

        public static bool AiPressureIsValid(int wAISensorBattery, int wAISensorPressure)
        {
            return ReverseEngineerAISensorMessageCodeInt_legacy(wAISensorBattery, wAISensorPressure) < 65519;
        }

        public static string ParseTankMessage(int sensorData)
        {
            return ParseTankMessage_verbose(sensorData);
        }

        public static string ParseTankMessage(int wAISensorBattery, int wAISensorPressure)
        {
            return ParseTankMessage_verbose(wAISensorBattery, wAISensorPressure);
        }

        public static string ParseTankMessage_concise(int wAISensorBattery, int wAISensorPressure)
        {
            return ParseTankMessage_concise(ReverseEngineerAISensorMessageCodeInt_legacy(wAISensorBattery, wAISensorPressure));
        }

        public static string ParseSacMessage(int sensorData)
        {
            return ParseSacMessage_verbose(sensorData);
        }

        public static string ParseSacMessage_verbose(float sac)
        {
            return ParseTankMessage_verbose(ReverseEngineerSacMessageCodeInt(sac));
        }

        public static string ParseSacMessage_concise(float sac)
        {
            return ParseTankMessage_concise(ReverseEngineerSacMessageCodeInt(sac));
        }

        public static string ParseTankMessage_verbose(int wAISensorBattery, int wAISensorPressure)
        {
            return ParseTankMessage_verbose(ReverseEngineerAISensorMessageCodeInt_legacy(wAISensorBattery, wAISensorPressure));
        }

        public static string ParseGasTimeMessage(int sensorData)
        {
            return ParseGasTimeMessage_verbose(sensorData);
        }

        public static string ParseTankMessage_concise(int sensorData)
        {
            switch (sensorData)
            {
                case 65535:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_ai_is_off");
                case 65534:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_no_comms");
                case 65533:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_no_comms");
                case 65532:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_not_paired");
                case 65519:
                case 65520:
                case 65521:
                case 65522:
                case 65523:
                case 65524:
                case 65525:
                case 65526:
                case 65527:
                case 65528:
                case 65529:
                case 65530:
                case 65531:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                default:
                    return "";
            }
        }

        public static string ParseTankMessage_verbose(int sensorData)
        {
            switch (sensorData)
            {
                case 0:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_unavailable_sensor_data");
                case 65535:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_ai_is_off");
                case 65534:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_no_comms_seconds_90");
                case 65533:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_no_comms_seconds_30");
                case 65532:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_transmitter_not_paired");
                case 65519:
                case 65520:
                case 65521:
                case 65522:
                case 65523:
                case 65524:
                case 65525:
                case 65526:
                case 65527:
                case 65528:
                case 65529:
                case 65530:
                case 65531:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                default:
                    return "";
            }
        }

        public static string ParseGasTimeMessage_concise(int sensorData)
        {
            switch (sensorData)
            {
                case 255:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_not_paired");
                case 254:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_no_comms");
                case 253:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_na_in_mode");
                case 252:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_na_deco");
                case 251:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.concise_no_tank_size");
                case 239:
                case 240:
                case 241:
                case 242:
                case 243:
                case 244:
                case 245:
                case 246:
                case 247:
                case 248:
                case 249:
                case 250:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                default:
                    return "";
            }
        }

        public static string ParseGasTimeMessage_verbose(int sensorData)
        {
            switch (sensorData)
            {
                case 255:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_not_paired");
                case 254:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_no_communication");
                case 253:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_na_in_current_mode");
                case 252:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_na_because_of_deco");
                case 251:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_tank_size_not_set_up");
                case 239:
                case 240:
                case 241:
                case 242:
                case 243:
                case 244:
                case 245:
                case 246:
                case 247:
                case 248:
                case 249:
                case 250:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                default:
                    return "";
            }
        }

        public static string ParseSacMessage_verbose(int sensorData)
        {
            switch (sensorData)
            {
                case 65535:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_ai_is_off");
                case 65534:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_no_comms_seconds_90");
                case 65533:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_na_in_current_mode");
                case 65532:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_transmitter_not_paired");
                case 65531:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_bad_setup");
                case 65530:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_not_diving");
                case 65529:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_waiting_for_initial_data");
                case 65528:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_sac_too_low");
                case 65527:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.verbose_gtr_sac_off");
                case 65520:
                case 65521:
                case 65522:
                case 65523:
                case 65524:
                case 65525:
                case 65526:
                    return ScriptLocalizationMod.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                default:
                    return "";
            }
        }
    }
}

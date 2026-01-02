using System.Collections.Generic;
using System.IO;
using System.Linq;
using DiveLogModels;
using ExtendedCoreParserUtilities;
using Newtonsoft.Json.Linq;
using ShearwaterUtils;
using static ShearwaterUtils.SettingDefinitions;

namespace Shearwater
{
    public static class ShearwaterUtilsExt
    {
        static ShearwaterUtilsExt()
        {
            DiveLogProductUtil.RetrieveFriendlyProductName = (int productId) =>
            {
                return $"Shearwater " + productId switch
                {
                    0 => "GF",
                    1 => "Pursuit",
                    2 => "Predator",
                    3 => "Petrel",
                    4 => "NERD",
                    5 => "Perdix",
                    6 => "Perdix AI",
                    7 => "NERD 2",
                    8 => "Teric",
                    9 => "Peregrine",
                    10 => "Petrel 3",
                    11 => "Perdix 2",
                    12 => "Tern",
                    13 => "Peregrine TX",
                    _ => "Unknown",
                };
            };
        }

        public static string GetComputerName(FinalLog finalLog)
        {
            return DiveLogProductUtil.FriendlyProductName(finalLog);
        }

        public static double? GetTankPressureInBar(DiveLogSample sample, int tankIndex, TankUnits tankUnit = TankUnits.Bar)
        {
            return tankIndex switch
            {
                0 => GetTankPressureInternal(sample, sample.WAISensor0Battery, sample.WAISensor0Pressure, tankUnit),
                1 => GetTankPressureInternal(sample, sample.WAISensor1Battery, sample.WAISensor1Pressure, tankUnit),
                2 => GetTankPressureInternal(sample, sample.WAISensor2Battery, sample.WAISensor2Pressure, tankUnit),
                3 => GetTankPressureInternal(sample, sample.WAISensor3Battery, sample.WAISensor3Pressure, tankUnit),
                _ => null,
            };
        }

        public static int? GetGasTimeRemainingInMinutes(DiveLogSample sample)
        {
            if (IsGasTimeRemainingValid(sample.WAISensorGasTimeRemaining))
            {
                return sample.WAISensorGasTimeRemaining;
            }

            return null;
        }

        public static double? GetSurfaceAirConsumptionInBar(DiveLogSample sample, TankUnits tankUnit = TankUnits.Bar)
        {

            if (IsTankPressureValid((int)(sample.Sac * 100f)) && IsSurfaceAirConsumptionValid(sample.sac_data))
            {
                return UnitConverter.ConvertTankUnits(sample.Sac, Settings.TankUnit);
            }

            return null;
        }

        private static double? GetTankPressureInternal(DiveLogSample sample, int aiSensorBattery, int aiSensorPressure, TankUnits tankUnit)
        {
            if (IsTankPressureValid(aiSensorBattery + aiSensorPressure / 2) && IsTankPressureValid(sample.sensor0_data))
            {
                return UnitConverter.ConvertTankUnits(aiSensorPressure, tankUnit);
            }

            return null;
        }

        public static bool IsTankPressureValid(int sensorData)
        {
            switch (sensorData)
            {
                case 0:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_unavailable_sensor_data");
                case 65535:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_ai_is_off");
                case 65534:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_no_comms_seconds_90");
                case 65533:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_no_comms_seconds_30");
                case 65532:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_transmitter_not_paired");
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
                    //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                    return false;
                default:
                    return true;
            }
        }

        private static bool IsGasTimeRemainingValid(int sensorData)
        {
            switch (sensorData)
            {
                case 255:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_not_paired");
                case 254:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_no_communication");
                case 253:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_na_in_current_mode");
                case 252:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_na_because_of_deco");
                case 251:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_tank_size_not_set_up");
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
                    //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                    return false;
                default:
                    return true;
            }
        }

        private static bool IsSurfaceAirConsumptionValid(int sensorData)
        {
            switch (sensorData)
            {
                case 65535:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_ai_is_off");
                case 65534:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_no_comms_seconds_90");
                case 65533:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_na_in_current_mode");
                case 65532:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_transmitter_not_paired");
                case 65531:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_bad_setup");
                case 65530:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_not_diving");
                case 65529:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_waiting_for_initial_data");
                case 65528:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_sac_too_low");
                case 65527:
                //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.verbose_gtr_sac_off");
                case 65520:
                case 65521:
                case 65522:
                case 65523:
                case 65524:
                case 65525:
                case 65526:
                    //return ShearwaterUtilsExt.GetLocalizedString("dive_computer.not_applicable_abbreviation");
                    return false;
                default:
                    return true;
            }
        }

    }
}

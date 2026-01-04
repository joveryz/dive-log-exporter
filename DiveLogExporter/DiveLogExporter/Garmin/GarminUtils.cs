using System.Collections.Generic;
using System.Reflection;
using Dynastream.Fit;

namespace DiveLogExporter.Garmin
{
    public static class GarminUtils
    {
        private static readonly Dictionary<ushort, string> GarminProductNames;


        static GarminUtils()
        {
            GarminProductNames = new Dictionary<ushort, string>();
            var fields = typeof(GarminProduct).GetFields(BindingFlags.Public | BindingFlags.Static);
            foreach (var field in fields)
            {
                if (field.IsLiteral && !field.IsInitOnly && field.FieldType == typeof(ushort))
                {
                    var value = (ushort)field.GetRawConstantValue();
                    GarminProductNames[value] = $"Garmin {field.Name}";
                }
            }
        }

        public static string GetComputerName(DeviceInfoMesg deviceInfo)
        {
            var id = deviceInfo.GetGarminProduct().Value;
            if (GarminProductNames.TryGetValue(id, out var productName))
            {
                return "Garmin " + productName;
            }

            return "Unknown";
        }

        public static string GetDiveMode(LapMesg garminLap)
        {
            switch (garminLap.GetSubSport())
            {
                case SubSport.SingleGasDiving:
                case SubSport.MultiGasDiving:
                    return "OC Rec";
                case SubSport.GaugeDiving:
                    return "Gauge";
                case SubSport.ApneaDiving:
                case SubSport.ApneaHunting:
                    return "Free Dive";
                default:
                    return "Unknown";
            }
        }
    }
}

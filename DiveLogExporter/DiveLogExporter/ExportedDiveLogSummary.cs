namespace DiveLogExporter
{
    internal class ExportedDiveLogSummary
    {
        // Summary Info

        public int Number { get; set; }

        public string Mode { get; set; }

        public string StartDate { get; set; }

        public string EndDate { get; set; }

        public int DurationInSeconds { get; set; }

        public double DepthInMetersMax { get; set; }

        public double DepthInMetersAvg { get; set; }

        public string Buddy { get; set; }

        public string Location { get; set; }

        public string Site { get; set; }

        public string Note { get; set; }

        // Environment Info

        public double TemperatureInCelsiusMax { get; set; }

        public double TemperatureInCelsiusMin { get; set; }

        public double TemperatureInCelsiusAvg { get; set; }

        public string Salinity { get; set; }

        public int SurfaceIntervalInSeconds { get; set; }

        public int SurfacePressureInMillibarPreDive { get; set; }

        public int SurfacePressureInMillibarPostDive { get; set; }

        // Deco Info
        public string DecoModel { get; set; }

        public int GradientFactorLow { get; set; }

        public int GradientFactorHigh { get; set; }

        public double GradientFactor99Max { get; set; }

        public double CNSPercentPreDive { get; set; }

        public double CNSPercentPostDive { get; set; }


        // Computer Info
        public string ComputerModel { get; set; }

        public string ComputerSerialNumber { get; set; }

        public int ComputerFirmwareVersion { get; set; }

        public string BatteryType { get; set; }

        public double BatteryVoltagePreDive { get; set; }

        public double BatteryVoltagePostDive { get; set; }

        public int SampleRateInMs { get; set; }

        public string DataFormat { get; set; }

        public int LogVersion { get; set; }

        public int DatabaseVersion { get; set; }

        // Others
        public bool O2SensorStatusPreDive { get; set; }

        public bool O2SensorStatusPostDive { get; set; }

        public int SensorDisplay { get; set; }

        public double PPO2SetpointLowPreDive { get; set; }

        public double PPO2SetpointLowPostDive { get; set; }

        public double PPO2SetpointHighPreDive { get; set; }

        public double PPO2SetpointHighPostDive { get; set; }

        public int Features { get; set; }
    }
}

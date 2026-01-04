namespace DiveLogExporter.Model
{
    public class GeneralDiveLogSummary
    {
        // Summary Info

        public int? Number { get; set; }

        public string? Mode { get; set; }

        public string? StartDate { get; set; }

        public string? EndDate { get; set; }

        public int? DurationInSeconds { get; set; }

        public string? Buddy { get; set; }

        public string? Location { get; set; }

        public string? Site { get; set; }

        public string? Note { get; set; }

        // Environment Info

        public double? DepthInMetersMax { get; set; }

        public double? DepthInMetersAvg { get; set; }

        public double? HeartRateMax { get; set; }

        public double? HeartRateMin { get; set; }

        public double? HeartRateAvg { get; set; }

        public double? TemperatureInCelsiusMax { get; set; }

        public double? TemperatureInCelsiusMin { get; set; }

        public double? TemperatureInCelsiusAvg { get; set; }

        public int? SurfacePressureInMillibarPreDive { get; set; }

        public int? SurfacePressureInMillibarPostDive { get; set; }

        public int? SurfaceIntervalInSeconds { get; set; }

        public int? WaterDenisity { get; set; }

        public string? WaterType { get; set; }

        // Computer Info
        public string? ComputerModel { get; set; }

        public string? ComputerSerialNumber { get; set; }

        public string? ComputerFirmwareVersion { get; set; }

        public string? BatteryType { get; set; }

        public double? BatteryVoltagePreDive { get; set; }

        public double? BatteryVoltagePostDive { get; set; }

        public int? SampleRateInMs { get; set; }

        public string? DataFormat { get; set; }


        // Optional Deco Info
        public string? DecoModel { get; set; }

        public int? GradientFactorLow { get; set; }

        public int? GradientFactorHigh { get; set; }

        public double? GradientFactorSurfaceEnd { get; set; }

        public double? CentralNervousSystemPercentPreDive { get; set; }

        public double? CentralNervousSystemPercentPostDive { get; set; }
    }
}

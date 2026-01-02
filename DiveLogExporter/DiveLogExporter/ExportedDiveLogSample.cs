namespace DiveLogExporter
{
    internal class ExportedDiveLogSample
    {
        public int Number { get; set; }

        public int ElapsedTimeInSeconds { get; set; }

        public object Depth { get; set; }

        public int TimeToSurfaceInMinutes { get; set; }

        public int TimeToSurfaceInMinutesAtPlusFive { get; set; }

        public int NoDecoLimit { get; set; }

        public int CNS { get; set; }

        public double GasDensity { get; set; }

        public int GradientFactor99 { get; set; }

        public float PPO2 { get; set; }

        public float PPN2 { get; set; }

        public float PPHE { get; set; }

        public double Tank1PressureInBar { get; set; }
        public double Tank2PressureInBar { get; set; }
        public double Tank3PressureInBar { get; set; }
        public double Tank4PressureInBar { get; set; }

        public double SAC { get; set; }

        public int Temperature { get; set; }

        public float BatteryVoltage { get; set; }

        public double GasTimeRemainingInMinutes { get; set; }
    }
}

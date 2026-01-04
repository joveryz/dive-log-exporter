using System.Collections.Generic;

namespace DiveLogExporter.Model
{
    public class GeneralDiveLog
    {
        public GeneralDiveLogSummary Summary { get; set; }

        public List<GeneralDiveLogTankInformation> Tanks { get; set; }

        public List<GeneralDiveLogSample> Samples { get; set; }

    }
}

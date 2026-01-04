using System.Collections.Generic;
using System.Linq;

namespace DiveLogExporter.Exporter
{
    public class ExporterFactory
    {
        private readonly List<IDiveLogExporter> _exporters = new List<IDiveLogExporter>();

        public ExporterFactory()
        {
            _exporters.Add(new ShearwaterDiveLogExporter());
            _exporters.Add(new GarminDiveLogExporter());
        }

        public IReadOnlyList<IDiveLogExporter> GetAllExporters() => _exporters.AsReadOnly();

        public IDiveLogExporter GetExporter(string inputPath)
        {
            return _exporters.FirstOrDefault(e => e.CanHandle(inputPath));
        }

        public IEnumerable<string> GetSupportedExtensions()
        {
            return _exporters.SelectMany(e => e.SupportedExtensions).Distinct();
        }
    }
}

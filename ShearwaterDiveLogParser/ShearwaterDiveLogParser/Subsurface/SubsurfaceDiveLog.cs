using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace ShearwaterDiveLogParser.Subsurface
{
    [XmlRoot("divelog")]
    public class DiveLog
    {
        [XmlAttribute("program")]
        public string Program { get; set; }

        [XmlAttribute("version")]
        public string Version { get; set; }

        [XmlElement("settings")]
        public Settings Settings { get; set; }

        [XmlElement("divesites")]
        public DiveSites DiveSites { get; set; }

        [XmlElement("dives")]
        public Dives Dives { get; set; }
    }

    public class Settings
    {
        [XmlElement("fingerprint")]
        public List<Fingerprint> Fingerprints { get; set; }
    }

    public class Fingerprint
    {
        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("serial")]
        public string Serial { get; set; }

        [XmlAttribute("deviceid")]
        public string DeviceId { get; set; }

        [XmlAttribute("diveid")]
        public string DiveId { get; set; }

        [XmlAttribute("data")]
        public string Data { get; set; }
    }

    public class DiveSites
    {
        // This can be extended based on the dive site structure if needed
    }

    public class Dives
    {
        [XmlElement("dive")]
        public List<Dive> DiveList { get; set; }
    }

    public class Dive
    {
        [XmlAttribute("number")]
        public int Number { get; set; }

        [XmlAttribute("date")]
        public string Date { get; set; }

        [XmlAttribute("time")]
        public string Time { get; set; }

        [XmlAttribute("duration")]
        public string Duration { get; set; }

        [XmlElement("divecomputer")]
        public DiveComputer DiveComputer { get; set; }
    }

    public class DiveComputer
    {
        [XmlAttribute("model")]
        public string Model { get; set; }

        [XmlAttribute("deviceid")]
        public string DeviceId { get; set; }

        [XmlAttribute("diveid")]
        public string DiveId { get; set; }

        [XmlAttribute("dctype")]
        public string DcType { get; set; }

        [XmlElement("depth")]
        public Depth Depth { get; set; }

        [XmlElement("temperature")]
        public Temperature Temperature { get; set; }

        [XmlElement("surface")]
        public Surface Surface { get; set; }

        [XmlElement("water")]
        public Water Water { get; set; }

        [XmlElement("extradata")]
        public List<ExtraData> ExtraDataList { get; set; }

        [XmlElement("sample")]
        public List<Sample> Samples { get; set; }
    }

    public class Depth
    {
        [XmlAttribute("max")]
        public string Max { get; set; }

        [XmlAttribute("mean")]
        public string Mean { get; set; }
    }

    public class Temperature
    {
        [XmlAttribute("water")]
        public string Water { get; set; }
    }

    public class Surface
    {
        [XmlAttribute("pressure")]
        public string Pressure { get; set; }
    }

    public class Water
    {
        [XmlAttribute("salinity")]
        public string Salinity { get; set; }
    }

    public class ExtraData
    {
        [XmlAttribute("key")]
        public string Key { get; set; }

        [XmlAttribute("value")]
        public string Value { get; set; }
    }

    public class Sample
    {
        [XmlAttribute("time")]
        public string Time { get; set; }

        [XmlAttribute("depth")]
        public string Depth { get; set; }

        [XmlAttribute("temp")]
        public string Temp { get; set; }
    }

}

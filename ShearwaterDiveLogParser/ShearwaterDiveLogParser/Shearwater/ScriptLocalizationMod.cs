using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ShearwaterDiveLogParser.Shearwater
{
    public static class ScriptLocalizationMod
    {
        private static readonly JObject LocalizationData;

        private static readonly Dictionary<string, string> LocalizationTerms;

        static ScriptLocalizationMod()
        {
            var json = File.ReadAllText("I2Languages.json");
            LocalizationData = JObject.Parse(json);
            var terms = LocalizationData["m_Structure"]["mSource"]["mTerms"].ToObject<List<JObject>>();
            LocalizationTerms = terms.ToDictionary(
                term => term["Term"].ToString(),
                term => term["Languages"].ToObject<List<string>>().FirstOrDefault()
            );
        }

        public static string GetLocalizedString(string key)
        {
            if (LocalizationTerms.TryGetValue(key.Replace(".", "/"), out var localizedString))
            {
                return localizedString;
            }

            throw new KeyNotFoundException($"Localization key '{key}' not found.");
        }
    }
}

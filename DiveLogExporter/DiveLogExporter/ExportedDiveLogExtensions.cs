using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DiveLogExporter
{
    internal static class ExportedDiveLogExtensions
    {
        public static string ToCsvHeader(this object obj)
        {
            var properties = obj.GetType().GetProperties().Select(p => p.Name).ToList();
            return string.Join(",", properties);
        }

        public static string ToCsvRow(this object obj)
        {
            var sb = new StringBuilder();
            var properties = obj.GetType().GetProperties();
            foreach (var property in properties)
            {
                var value = property.GetValue(obj);
                if (value != null)
                {
                    var stringValue = value.ToString();
                    if (stringValue.Contains(",") || stringValue.Contains("\"") || stringValue.Contains("\n"))
                    {
                        stringValue = "\"" + stringValue.Replace("\"", "\"\"") + "\"";
                    }
                    sb.Append(stringValue);
                }
                sb.Append(",");
            }
            if (sb.Length > 0)
            {
                sb.Length--; // Remove the last comma
            }
            return sb.ToString();
        }

        public static string ToCsvRows(this IEnumerable<object> objs)
        {
            var sb = new StringBuilder();
            foreach (var obj in objs)
            {
                sb.AppendLine(ToCsvRow(obj));
            }
            return sb.ToString().TrimEnd(Environment.NewLine.ToCharArray());
        }
    }
}

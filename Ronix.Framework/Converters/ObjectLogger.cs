using System;
using System.Reflection;
using System.Text;

namespace Ronix.Framework.Converters
{
    public static class ObjectLogger
    {
        public static string AsString(object obj)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"{obj.GetType()} {obj}");
            foreach (var property in obj.GetType()
                        .GetProperties(BindingFlags.Instance | BindingFlags.FlattenHierarchy | BindingFlags.Public | BindingFlags.NonPublic))
            {
                string val;
                try
                {
                    val = property.GetValue(obj)?.ToString() ?? "null";
                }
                catch (Exception e)
                {
                    val = $"[{e.GetType()} on getter: {e.Message}]";
                }
                sb.AppendLine($"{property.Name}:\t{val}");
            }
            return sb.ToString();
        }
    }
}

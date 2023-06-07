using System;
using System.Text;

namespace Ronix.Framework.Extensions
{
    public static class ExceptionExtensions
    {
        public static string GetDescription(this Exception @this)
        {
            if (@this == null)
                return "null";
            var sb = new StringBuilder();
            WriteDescription(@this, sb, string.Empty);
            return sb.ToString();
        }

        private static void WriteDescription(Exception ex, StringBuilder sb, string prefix)
        {
            sb.Append(prefix);
            sb.AppendLine(ex.GetType().FullName);
            sb.Append(prefix);
            sb.AppendLine(ex.Message);
            sb.Append(prefix);
            sb.AppendLine(ex.StackTrace ?? "[NO STACK TRACE]");
            var nextPrefix = "    " + prefix;
            if (ex.InnerException != null)
            {
                sb.Append(prefix);
                sb.AppendLine("Inner exception:");
                WriteDescription(ex.InnerException, sb, nextPrefix);
            }
            var aggr = ex as AggregateException;
            if (aggr != null)
            {
                sb.Append(prefix);
                sb.AppendLine("Aggregated exceptions:");
                var index = 0;
                foreach (var innerException in aggr.InnerExceptions)
                {
                    sb.Append(prefix);
                    sb.AppendLine("[" + index++ + "]:");
                    WriteDescription(innerException, sb, nextPrefix);
                }
            }
        }
    }
}

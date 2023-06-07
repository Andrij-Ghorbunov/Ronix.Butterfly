using System;
using System.Globalization;

namespace Ronix.Framework.Mvvm
{
    /// <summary>
    /// Provides methods to perform 'obvious' conversions, like string "123" -> int 123.
    /// </summary>
    public static class DefaultConverter
    {
        /// <summary>
        /// Converts an object into the desired type or throws an exception if it cannot be converted.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <returns>The converted object.</returns>
        public static T Convert<T>(object obj)
        {
            if (obj is T)
                return (T)obj;
            if (obj == null)
                return default(T);
            var type = typeof(T);
            if (type == typeof(string))
                return Convert<T>(obj.ToString());
            if (obj is string)
            {
                var str = obj as string;
                if (type == typeof(uint))
                    return Convert<T>(UInt32.Parse(str));
                if (type == typeof(int))
                    return Convert<T>(Int32.Parse(str));
                if (type == typeof(ulong))
                    return Convert<T>(UInt64.Parse(str));
                if (type == typeof(long))
                    return Convert<T>(Int64.Parse(str));
                if (type == typeof(ushort))
                    return Convert<T>(UInt16.Parse(str));
                if (type == typeof(short))
                    return Convert<T>(Int16.Parse(str));
                if (type == typeof(byte))
                    return Convert<T>(Byte.Parse(str));
                if (type == typeof(sbyte))
                    return Convert<T>(SByte.Parse(str));
                if (type == typeof(float))
                    return Convert<T>(Single.Parse(str, NumberStyles.Any));
                if (type == typeof(double))
                    return Convert<T>(Double.Parse(str, NumberStyles.Any));
                if (type == typeof(char))
                    return Convert<T>(Char.Parse(str));
                if (type == typeof(bool))
                    return Convert<T>(Boolean.Parse(str));
                if (type.IsEnum)
                {
                    try
                    {
                        return Convert<T>(Enum.Parse(type, str));
                    }
                    catch (Exception)
                    {
                        return Convert<T>(Enum.Parse(type, str, true));
                    }
                }
                return (T)obj;
            }
            return Convert<T>(obj.ToString());
        }

        /// <summary>
        /// Converts an object into the desired type. The return value specifies whether the conversion was successful.
        /// </summary>
        /// <typeparam name="T">The target type.</typeparam>
        /// <param name="obj">The object to convert.</param>
        /// <param name="result">The covnerted object.</param>
        /// <returns>True if the conversion was successful; otherwise, false.</returns>
        public static bool TryConvert<T>(object obj, out T result)
        {
            try
            {
                result = Convert<T>(obj);
                return true;
            }
            catch (Exception)
            {
                result = default(T);
                return false;
            }
        }
    }
}

using System.Collections.Generic;
using System.Text.RegularExpressions;

namespace System.StringFormats
{
    public static class StringFormat
    {

        public static IFormatProvider formatProvider;
        static IFormatProvider FormatProvider
        {
            get
            {
                if (formatProvider == null)
                {
                    formatProvider =
                        new CallStringFormatProvider(
                        new RegexStringFormatProvider(
                        new NameStringFormatProvider()));
                }
                return formatProvider;
            }
        }

        public static string FormatString(this string format, params object[] args)
        {
            return FormatString(format, FormatProvider, args);
        }


        public static string FormatString(this string format, IFormatProvider formatProvider, params object[] args)
        {

            string result;

            result = indexFormatStringRegex.Replace(format, (m) =>
            {
                string indexStr = m.Groups["index"].Value;
                int index;
                object value;
                string ret = null;


                if (string.IsNullOrEmpty(indexStr))
                    throw new FormatException("format error:" + m.Value);
                if (!int.TryParse(indexStr, out index))
                    throw new Exception("format index error. " + m.Value);

                if (index < 0 || index >= args.Length)
                    throw new Exception("format index error. " + m.Value);
                value = args[index];

                if (value != null)
                {
                    object val;
                    val = value;
                    string formats = m.Groups["format"].Value;

                    foreach (var fmt in formats.Split(':'))
                    {
                        ret = string.Format(formatProvider, "{0:" + fmt + "}", val);
                        val = ret;
                    }
                }
                else
                {
                    ret = string.Empty;
                }

                return ret;
            });

            return result;
        }


        #region Format String Key

        private static Regex indexFormatStringRegex = new Regex("(?<!\\{)\\{(?<index>\\d+)(:(?<format>[^}]*))?\\}(?!\\})");
        private static Regex keyFormatStringRegex = new Regex("(?<!\\{)\\{\\$(?<key>[^}:]*)(:(?<format>[^}]*))?\\}(?!\\})");


        /// <summary>
        /// format:{$key:format} 
        /// </summary>
        public static string FormatStringWithKey(this string format, Dictionary<string, object> values)
        {
            return FormatStringWithKey(format, new DictionaryFormatStringValueProvider(values));
        }

        /// <summary>
        /// format:{$key:format} 
        /// </summary>
        public static string FormatStringWithKey(this string format, IKeyFormatStringValueProvider valueProvider)
        {
            return FormatStringWithKey(format, FormatProvider, valueProvider);
        }

        /// <summary>
        /// format:{$key:format} 
        /// </summary>
        public static string FormatStringWithKey(this string format, IFormatProvider formatProvider, IKeyFormatStringValueProvider valueProvider)
        {
            string result;

            result = keyFormatStringRegex.Replace(format, (m) =>
            {
                string paramName = m.Groups["key"].Value;
                object value;
                string ret = null;


                if (string.IsNullOrEmpty(paramName))
                    throw new FormatException("format error:" + m.Value);

                value = valueProvider.GetFormatValue(paramName);

                if (value != null)
                {
                    object val;
                    val = value;
                    string formats = m.Groups["format"].Value;

                    foreach (var fmt in formats.Split(':'))
                    {
                        ret = string.Format(formatProvider, "{0:" + fmt + "}", val);
                        val = ret;
                    }
                }
                else
                {
                    ret = string.Empty;
                }

                return ret;
            });
            return result;
        }

        #endregion
    }
}

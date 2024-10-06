using System;
using System.Globalization;
using System.Linq;
using System.Text;

namespace UTO.Framework.Shared.Extensions
{
    public static class StringExtensions
    {
        public static string UriCombine(this string value, string append)
        {
            if (String.IsNullOrEmpty(value)) return append;
            if (String.IsNullOrEmpty(append)) return value;
            return value.TrimEnd('/') + "/" + append.TrimStart('/');
        }

        public static string ToSafeString(this object obj)
        {
            return obj != null ? obj.ToString() : String.Empty;
        }

        public static T ToUpper<T>(this T obj)
        {
            var properties = obj.GetType().GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
            var count = properties.Count();

            for (int i = 0; i < count; i++)
            {
                if (String.Compare(properties[i].PropertyType.Name, "string", StringComparison.OrdinalIgnoreCase) == 0)
                {
                    var value = properties[i].GetValue(obj, null).ToSafeString();
                    properties[i].SetValue(obj, value.ToUpper(CultureInfo.InvariantCulture));
                }
            }
            return obj;
        }

        public static string Repeat(this string str, int count)
        {
            return new StringBuilder().Insert(0, str, count).ToString();
        }

        public static bool IsEscaped(this string str, int index)
        {
            bool escaped = false;
            while (index > 0 && str[--index] == '\\') escaped = !escaped;
            return escaped;
        }

        public static bool IsEscaped(this StringBuilder str, int index)
        {
            return str.ToString().IsEscaped(index);
        }
    }
}

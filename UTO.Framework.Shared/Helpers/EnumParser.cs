using System;

namespace UTO.Framework.Shared.Helpers
{
    public static class EnumParser
    {
        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }
    }
}

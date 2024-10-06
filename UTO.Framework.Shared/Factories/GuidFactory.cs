using System;

namespace UTO.Framework.Shared.Factories
{
    public static class GuidFactory
    {
        public static string Create(bool base64Format = true)
        {
            if (base64Format)
            {
                return Convert.ToBase64String(Guid.NewGuid().ToByteArray());
            }
            else
            {
                return Guid.NewGuid().ToString();
            }
        }

        public static string Create()
        {
            return Create(true);
        }
    }
}

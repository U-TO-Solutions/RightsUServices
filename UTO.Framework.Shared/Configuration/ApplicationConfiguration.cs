using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using UTO.Framework.Shared.Guards;
using UTO.Framework.Shared.Interfaces;

namespace UTO.Framework.Shared.Configuration
{
    public class ApplicationConfiguration : IConfiguration
    {
        public T GetConfigurationValue<T>(string key)
        {
            ParameterGuard.AgainstNullStringParameter(key);
            return GetConfigurationValue(key, default(T), true);
        }

        public string GetConfigurationValue(string key)
        {
            ParameterGuard.AgainstNullStringParameter(key);
            return GetConfigurationValue<string>(key);
        }

        public T GetConfigurationValue<T>(string key, T defaultValue)
        {
            ParameterGuard.AgainstNullStringParameter(key);
            return GetConfigurationValue(key, defaultValue, false);
        }

        public string GetConfigurationValue(string key, string defaultValue)
        {
            ParameterGuard.AgainstNullStringParameter(key);
            return GetConfigurationValue<string>(key, defaultValue);
        }

        public string GetConnectionString(string name)
        {
            ParameterGuard.AgainstNullStringParameter(name);
            return this.GetConnectionString<string>(name, string.Empty);
        }

        public T GetConnectionString<T>(string name, T defaultValue)
        {
            ParameterGuard.AgainstNullStringParameter(name);

            if (ConfigurationManager.ConnectionStrings[name] == null)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.CurrentCulture, "Web.Config file must contains key {0} with valid value.", name));
            }

            string val = ConfigurationManager.ConnectionStrings[name].ConnectionString;

            if (!string.IsNullOrWhiteSpace(val))
            {
                return (T)Convert.ChangeType(val, typeof(T), CultureInfo.InvariantCulture);
            }

            return defaultValue;
        }

        public string GetProviderFromEFConnection(string connectionString)
        {
            throw new NotImplementedException();
        }

        private T GetConfigurationValue<T>(string key, T defaultValue, bool throwException)
        {
            ParameterGuard.AgainstNullStringParameter(key);

            var value = ConfigurationManager.AppSettings[key];
            if (value == null)
            {
                if (throwException)
                    throw new KeyNotFoundException("Key " + key + " not found.");
                return defaultValue;
            }
            try
            {
                if (typeof(Enum).IsAssignableFrom(typeof(T)))
                    return (T)Enum.Parse(typeof(T), value);
                return (T)Convert.ChangeType(value, typeof(T));
            }
            catch (Exception ex)
            {
                if (throwException)
                    throw;
                return defaultValue;
            }
        }
    }
}

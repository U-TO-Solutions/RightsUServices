namespace UTO.Framework.Shared.Interfaces
{
    public interface IConfiguration
    {
        T GetConfigurationValue<T>(string key);
        string GetConfigurationValue(string key);
        T GetConfigurationValue<T>(string key, T defaultValue);
        string GetConfigurationValue(string key, string defaultValue);
        string GetConnectionString(string connectionString);
        string GetProviderFromEFConnection(string connectionString);
    }
}

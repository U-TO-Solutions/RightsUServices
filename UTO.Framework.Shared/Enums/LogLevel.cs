namespace UTO.Framework.Shared.Enums
{
    /// <summary>
    /// Log Level
    /// default value in production should be equal to or above Information
    /// https://docs.microsoft.com/en-us/dotnet/api/microsoft.extensions.logging.loglevel?view=aspnetcore-3.0
    /// </summary>
    public enum LogLevel
    {
        Trace = 0,
        Debug = 1,
        Information = 2,
        Warning = 3,
        Error = 4,
        Critical = 5,
        None = 6
    }
}

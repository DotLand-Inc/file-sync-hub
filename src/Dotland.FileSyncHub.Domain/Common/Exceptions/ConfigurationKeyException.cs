namespace Dotland.FileSyncHub.Domain.Common.Exceptions;

public class ConfigurationKeyException(string key) : Exception
{
    public string Key { get;  } = key;
    public override string Message { get; } = $"The configuration key ({ key }) was not found.";
    public string? CustomMessage { get; } = null;

    public ConfigurationKeyException(string key, string message) : this(key)
    {
        CustomMessage  = message;
    }
}
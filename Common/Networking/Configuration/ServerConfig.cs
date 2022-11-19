namespace Common.Networking.Configuration;

public sealed class ServerConfig
{
    public ushort ServerPort { get; set; }
    
    public ushort ClientVersion { get; set; }
    public string ClientPatchVersion { get; set; } = "";
    public byte ClientLocale { get; set; }
}
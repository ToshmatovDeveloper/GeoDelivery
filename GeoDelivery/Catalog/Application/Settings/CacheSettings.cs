namespace Catalog.Application.Settings;

public class CacheSettings
{
    public const string SectionName = "CacheSettings";

    public TimeSpan TimeToLive { get; set; }
}
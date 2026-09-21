namespace Catalog.Application.Caching;

public interface ICachableQuery
{
    string CacheKey { get; }
    TimeSpan? Expiration => null; 
}
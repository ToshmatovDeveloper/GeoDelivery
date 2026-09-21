using Catalog.Infrastructure.Caching;
using MediatR;

namespace Catalog.Application.Caching;

public class CachingBehavior<TRequest, TResponse>(
    RedisCacheService cacheService,
    ILogger<CachingBehavior<TRequest, TResponse>> logger) 
    : IPipelineBehavior<TRequest, TResponse>
    where TRequest : notnull
{
    public async Task<TResponse> Handle(
        TRequest request,
        RequestHandlerDelegate<TResponse> next,
        CancellationToken cancellationToken)
    {
        if (request is not ICachableQuery cachableQuery)
        {
            return await next();
        }

        var cachedResponse = await cacheService.GetAsync<TResponse>(cachableQuery.CacheKey);
        if (cachedResponse is not null)
        {
            return cachedResponse;
        }

        var response = await next();

        await cacheService.SetAsync(cachableQuery.CacheKey, response, cachableQuery.Expiration);

        return response;
    }
}
using EcoMotorsPractice.Application.Common.Caching;

namespace EcoMotorsPractice.Infrastructure.Caching;

public class CacheKeyService : ICacheKeyService
{

    public CacheKeyService() { }

    public string GetCacheKey(string name, object id)
    {
        return $"{name}-{id}";
    }
}
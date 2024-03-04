using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;

namespace Infrastructure.Test.Caching;

public class LocalCacheService : CacheService<EcoMotorsPractice.Infrastructure.Caching.LocalCacheService>
{
    protected override EcoMotorsPractice.Infrastructure.Caching.LocalCacheService CreateCacheService() =>
        new(
            new MemoryCache(new MemoryCacheOptions()),
            NullLogger<EcoMotorsPractice.Infrastructure.Caching.LocalCacheService>.Instance);
}
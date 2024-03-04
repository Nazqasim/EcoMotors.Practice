using EcoMotorsPractice.Infrastructure.Common.Services;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;

namespace Infrastructure.Test.Caching;

public class DistributedCacheService : CacheService<EcoMotorsPractice.Infrastructure.Caching.DistributedCacheService>
{
    protected override EcoMotorsPractice.Infrastructure.Caching.DistributedCacheService CreateCacheService() =>
        new(
            new MemoryDistributedCache(Options.Create(new MemoryDistributedCacheOptions())),
            new NewtonSoftService(),
            NullLogger<EcoMotorsPractice.Infrastructure.Caching.DistributedCacheService>.Instance);
}
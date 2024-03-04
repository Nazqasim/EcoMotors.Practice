using EcoMotorsPractice.Application.Common.Events;
using EcoMotorsPractice.Application.Common.Interfaces;
using EcoMotorsPractice.Domain.Identity;
using EcoMotorsPractice.Infrastructure.Persistence.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace EcoMotorsPractice.Infrastructure.Persistence.Context;

public class ApplicationDbContext : BaseDbContext
{
    public ApplicationDbContext(DbContextOptions options, ICurrentUser currentUser, ISerializerService serializer, IOptions<DatabaseSettings> dbSettings, IEventPublisher events)
        : base(options, currentUser, serializer, dbSettings, events)
    {
    }

    // Schema: Identity
    public DbSet<ReferentialUser> ReferentialUsers => Set<ReferentialUser>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.HasDefaultSchema(SchemaNames.Common);
    }
}
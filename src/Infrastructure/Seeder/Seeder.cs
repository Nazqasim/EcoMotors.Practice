using EcoMotorsPractice.Application.Common.Interfaces;
using EcoMotorsPractice.Infrastructure.Persistence;
using EcoMotorsPractice.Infrastructure.Persistence.Context;
using EcoMotorsPractice.Infrastructure.Persistence.Initialization;
using EcoMotorsPractice.Infrastructure.SqlAdoHelper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using EcoMotorsPractice.Infrastructure.Helpers;

namespace EcoMotorsPractice.Infrastructure.Seeder;
internal class Seeder : ICustomSeeder
{
    private readonly ISerializerService _serializerService;
    private readonly ApplicationDbContext _db;
    private readonly ILogger<Seeder> _seeder;
    private readonly string? _path;
    private readonly IOptions<DatabaseSettings> _settings;

    public Seeder(
        ISerializerService serializerService,
        ILogger<Seeder> seeder,
        IOptions<DatabaseSettings> settings,

        ApplicationDbContext db)
    {
        _serializerService = serializerService;
        _seeder = seeder;
        _db = db;
        _settings = settings;
        _path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
    }

    public async Task InitializeAsync(CancellationToken cancellationToken)
    {
        await StoredProcedureSeedingAsync(cancellationToken);
    }

    public async Task StoredProcedureSeedingAsync(CancellationToken cancellationToken)
    {
        string[] sqlFiles = Directory.GetFiles(_path + "/Scripts", "*.sql");

        foreach (string file in sqlFiles)
        {
            string contents = File.ReadAllText(file);
            using (var helper = new SQLAdoHelper(_settings))
            {
                helper.ExecNonQuery(contents);
            }
        }

        _seeder.LogInformation("Seeded Stored Procedures.");
    }
}

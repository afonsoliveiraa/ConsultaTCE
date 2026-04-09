using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

var factory = new AppDbContextFactory();

try
{
    await using var context = factory.CreateDbContext(args);

    var applied = await context.Database.GetAppliedMigrationsAsync();
    var pending = await context.Database.GetPendingMigrationsAsync();

    Console.WriteLine("Applied migrations:");
    foreach (var migration in applied)
    {
        Console.WriteLine($"- {migration}");
    }

    Console.WriteLine("Pending migrations:");
    foreach (var migration in pending)
    {
        Console.WriteLine($"- {migration}");
    }

    await context.Database.MigrateAsync();
    Console.WriteLine("Database update completed.");
}
catch (Exception exception)
{
    Console.Error.WriteLine(exception.ToString());
    Environment.ExitCode = 1;
}

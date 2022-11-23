// See https://aka.ms/new-console-template for more information
using System.Diagnostics.Tracing;
using System.Xml.Linq;


var random = new Random(0);
Task runMigration = new Task(() =>
{
    while (true)
    {
        Thread.Sleep(1000);

        MigratedRecordsCounterSource.Log.Add(random.Next(1,5));
    }

});

runMigration.Start();

Console.ReadLine();



[EventSource(Name = "Migrator.MigratedRecordsCounter")]
public sealed class MigratedRecordsCounterSource : EventSource
{
    public static readonly MigratedRecordsCounterSource Log = new();
    private PollingCounter _migratedRecordsCounter;
    private long _migratedRecordsCount = 0;

    private MigratedRecordsCounterSource()
    {
        _migratedRecordsCounter = new PollingCounter("migrated-records-count", this, () => Interlocked.Read(ref _migratedRecordsCount))
        {
            DisplayName = "Migration Count"
        };
    }

    //See https://learn.microsoft.com/en-us/dotnet/core/diagnostics/event-counters#concurrency
    public void Add(int count) => Interlocked.Add(ref _migratedRecordsCount, count);

    protected override void Dispose(bool disposing)
    {
        _migratedRecordsCounter?.Dispose();
        _migratedRecordsCounter = null;

        base.Dispose(disposing);
    }
}

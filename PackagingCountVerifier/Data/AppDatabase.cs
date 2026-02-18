using SQLite;
using PackagingCountVerifier.Data.Entities;

namespace PackagingCountVerifier.Data;

public class AppDatabase
{
    private readonly SQLiteAsyncConnection _db;

    public AppDatabase(string dbPath)
    {
        _db = new SQLiteAsyncConnection(dbPath);

        _db.CreateTableAsync<PackingJobEntity>().Wait();
        _db.CreateTableAsync<BoxHistoryEntity>().Wait();
    }

    public Task InsertJobAsync(PackingJobEntity job) =>
        _db.InsertAsync(job);

    public Task InsertBoxAsync(BoxHistoryEntity box) =>
        _db.InsertAsync(box);

    public Task<List<PackingJobEntity>> GetJobsAsync() =>
        _db.Table<PackingJobEntity>().OrderByDescending(x => x.CompletedAt).ToListAsync();
}

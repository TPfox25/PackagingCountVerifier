using SQLite;

namespace PackagingCountVerifier.Data.Entities;

public class BoxHistoryEntity
{
    [PrimaryKey]
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public Guid PackingJobId { get; set; }

    public int BoxNumber { get; set; }
    public int ItemsInBox { get; set; }

    public string PackedAt { get; set; } = string.Empty;
}

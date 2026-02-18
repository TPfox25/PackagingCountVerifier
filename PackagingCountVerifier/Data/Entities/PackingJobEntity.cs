using SQLite;

namespace PackagingCountVerifier.Data.Entities
{
    public class PackingJobEntity
    {
        [PrimaryKey]
        public Guid Id { get; set; }

        public string JobName { get; set; }
        public string ItemType { get; set; }
        public int ExpectedTotal { get; set; }
        public int ItemsPerBox { get; set; }
        public int ExpectedBoxes { get; set; }
        public int PackedTotal { get; set; }
        public int BoxesCompleted { get; set; }
        public string StartedAt { get; set; }
        public string CompletedAt { get; set; }
        public string Status { get; set; }
    }
}

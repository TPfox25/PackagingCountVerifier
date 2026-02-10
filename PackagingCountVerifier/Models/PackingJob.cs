using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PackagingCountVerifier.Models
{
    public class PackingJob : INotifyPropertyChanged
    {
        string jobName;
        string itemType;
        int expectedTotal;
        int itemsPerBox;
        int packedTotal;
        int boxesCompleted;
        int currentBoxCount;

        public event PropertyChangedEventHandler? PropertyChanged;
        void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        // 🔹 METADATA (used by MainPage)
        public string JobName
        {
            get => jobName;
            set { jobName = value; OnPropertyChanged(); }
        }

        public string ItemType
        {
            get => itemType;
            set { itemType = value; OnPropertyChanged(); }
        }

        // 🔹 PACKING RULES
        public int ExpectedTotal
        {
            get => expectedTotal;
            set { expectedTotal = value; OnPropertyChanged(); OnPropertyChanged(nameof(ExpectedBoxes)); }
        }

        public int ItemsPerBox
        {
            get => itemsPerBox;
            set { itemsPerBox = value; OnPropertyChanged(); OnPropertyChanged(nameof(ExpectedBoxes)); }
        }

        public int ExpectedBoxes =>
            ItemsPerBox <= 0 ? 0 : (int)Math.Ceiling((double)ExpectedTotal / ItemsPerBox);

        // 🔹 LIVE STATE
        public int PackedTotal
        {
            get => packedTotal;
            set { packedTotal = value; OnPropertyChanged(); }
        }

        public int BoxesCompleted
        {
            get => boxesCompleted;
            set { boxesCompleted = value; OnPropertyChanged(); }
        }

        public int CurrentBoxCount
        {
            get => currentBoxCount;
            set { currentBoxCount = value; OnPropertyChanged(); }
        }

        public int RemainingItems =>
            Math.Max(0, ExpectedTotal - PackedTotal);

        public bool IsJobComplete =>
            PackedTotal == ExpectedTotal &&
            BoxesCompleted == ExpectedBoxes;
    }
}

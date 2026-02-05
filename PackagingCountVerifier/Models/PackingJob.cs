using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace PackagingCountVerifier.Models
{
    public class PackingJob : INotifyPropertyChanged
    {
        private string jobName;
        private string itemType;
        private int expectedTotal;
        private int itemsPerBox;
        private int packedTotal;
        private int boxesCompleted;
        private int currentBoxCount;   // ✅ ADDED

        public event PropertyChangedEventHandler? PropertyChanged;

        void OnPropertyChanged([CallerMemberName] string? name = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        public string JobName
        {
            get => jobName;
            set
            {
                jobName = value;
                OnPropertyChanged();
            }
        }

        public string ItemType
        {
            get => itemType;
            set
            {
                itemType = value;
                OnPropertyChanged();
            }
        }

        public int ExpectedTotal
        {
            get => expectedTotal;
            set
            {
                expectedTotal = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ExpectedBoxes));
            }
        }

        public int ItemsPerBox
        {
            get => itemsPerBox;
            set
            {
                itemsPerBox = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(ExpectedBoxes));
            }
        }

        public int ExpectedBoxes =>
            ItemsPerBox <= 0 ? 0 : ExpectedTotal / ItemsPerBox;

        public int CurrentBoxCount
        {
            get => currentBoxCount;
            set
            {
                currentBoxCount = value;
                OnPropertyChanged();
            }
        }

        public int PackedTotal
        {
            get => packedTotal;
            set
            {
                packedTotal = value;
                OnPropertyChanged();
            }
        }

        public int BoxesCompleted
        {
            get => boxesCompleted;
            set
            {
                boxesCompleted = value;
                OnPropertyChanged();
            }
        }

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

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
        private int currentBoxCount;

        // 🔁 Undo support
        private int lastAddedQuantity;

        public event PropertyChangedEventHandler? PropertyChanged;

        private void OnPropertyChanged([CallerMemberName] string? name = null)
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
                OnPropertyChanged(nameof(CanFinishPacking));
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
                OnPropertyChanged(nameof(CanFinishPacking));
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
                OnPropertyChanged(nameof(CanFinishPacking));
            }
        }

        public int BoxesCompleted
        {
            get => boxesCompleted;
            set
            {
                boxesCompleted = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanFinishPacking));
            }
        }

        // 🔁 Tracks last add (for Undo)
        public int LastAddedQuantity
        {
            get => lastAddedQuantity;
            set
            {
                lastAddedQuantity = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(CanUndoLastAdd));
            }
        }

        public bool CanUndoLastAdd => LastAddedQuantity > 0;

        public bool CanFinishPacking =>
            PackedTotal == ExpectedTotal &&
            BoxesCompleted == ExpectedBoxes;

        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}

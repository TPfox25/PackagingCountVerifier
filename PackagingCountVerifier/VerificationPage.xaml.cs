using PackagingCountVerifier.Models;
using PackagingCountVerifier.Data;
using PackagingCountVerifier.Data.Entities;

namespace PackagingCountVerifier;

public partial class VerificationPage : ContentPage
{
    private readonly PackingJob job;
    private readonly AppDatabase _db;

    public VerificationPage(PackingJob job, AppDatabase db)
    {
        InitializeComponent();

        this.job = job;
        _db = db;

        SaveJobToDatabase();
        Verify();
    }

    private async void SaveJobToDatabase()
    {
        await _db.InsertJobAsync(new PackingJobEntity
        {
            Id = Guid.NewGuid(), // Unique record ID
            JobName = job.JobName,
            ItemType = job.ItemType,
            ExpectedTotal = job.ExpectedTotal,
            ItemsPerBox = job.ItemsPerBox,
            ExpectedBoxes = job.ExpectedBoxes,
            PackedTotal = job.PackedTotal,
            BoxesCompleted = job.BoxesCompleted,
            StartedAt = job.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            CompletedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Status = job.PackedTotal == job.ExpectedTotal ? "Completed" : "Mismatch"
        });
    }

    private void Verify()
    {
        bool totalMatch = job.PackedTotal == job.ExpectedTotal;
        bool boxMatch = job.BoxesCompleted == job.ExpectedBoxes;

        if (totalMatch && boxMatch)
        {
            ResultLabel.Text =
                "COUNT SUCCESSFUL\nAll items accounted for.";
            ResultLabel.TextColor = Colors.Green;
        }
        else
        {
            int difference = job.ExpectedTotal - job.PackedTotal;

            if (difference > 0)
            {
                ResultLabel.Text =
                    $"COUNT MISMATCH\nShort by {difference} items";
            }
            else
            {
                ResultLabel.Text =
                    $"COUNT MISMATCH\nOver by {Math.Abs(difference)} items";
            }

            ResultLabel.TextColor = Colors.Red;
        }
    }

    private void OnCloseAppClicked(object sender, EventArgs e)
    {
#if ANDROID
        Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
#elif WINDOWS
        Application.Current?.Quit();
#else
        Application.Current?.Quit();
#endif
    }

    private async void OnNewJobClicked(object sender, EventArgs e)
    {
        bool confirm = await DisplayAlert(
            "Start New Job",
            "This will start a new job.",
            "New Job",
            "Cancel");

        if (!confirm) return;

        var freshJob = new PackingJob();

        Application.Current!.MainPage =
            new NavigationPage(new MainPage(freshJob));
    }
}

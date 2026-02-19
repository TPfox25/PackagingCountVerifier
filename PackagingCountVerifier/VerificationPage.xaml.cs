using PackagingCountVerifier.Models;
using PackagingCountVerifier.Data;
using PackagingCountVerifier.Data.Entities;

namespace PackagingCountVerifier;

public partial class VerificationPage : ContentPage
{
    private readonly PackingJob job;
    private readonly AppDatabase _db;

    private bool _isMismatch;

    public VerificationPage(PackingJob job, AppDatabase db)
    {
        InitializeComponent();

        this.job = job;
        _db = db;

        Verify();
        SaveJobToDatabase();
    }

    private async void SaveJobToDatabase()
    {
        await _db.InsertJobAsync(new PackingJobEntity
        {
            Id = Guid.NewGuid(),
            JobName = job.JobName,
            ItemType = job.ItemType,
            ExpectedTotal = job.ExpectedTotal,
            ItemsPerBox = job.ItemsPerBox,
            ExpectedBoxes = job.ExpectedBoxes,
            PackedTotal = job.PackedTotal,
            BoxesCompleted = job.BoxesCompleted,
            StartedAt = job.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
            CompletedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Status = _isMismatch ? "Mismatch" : "Completed"
        });
    }

    private void Verify()
    {
        bool totalMatch = job.PackedTotal == job.ExpectedTotal;
        bool boxMatch = job.BoxesCompleted == job.ExpectedBoxes;

        _isMismatch = !(totalMatch && boxMatch);

        if (!_isMismatch)
        {
            ResultLabel.Text =
                "? COUNT SUCCESSFUL\n\n" +
                $"Total: {job.PackedTotal}/{job.ExpectedTotal}\n" +
                $"Boxes: {job.BoxesCompleted}/{job.ExpectedBoxes}";

            ResultLabel.TextColor = Colors.Green;
        }
        else
        {
            int difference = job.ExpectedTotal - job.PackedTotal;

            string issue =
                difference > 0
                    ? $"SHORTAGE: {difference} items"
                    : difference < 0
                        ? $"OVERPACK: {Math.Abs(difference)} items"
                        : "Box count mismatch";

            ResultLabel.Text =
                "? COUNT VERIFICATION FAILED\n\n" +
                issue + "\n\n" +
                $"Total: {job.PackedTotal}/{job.ExpectedTotal}\n" +
                $"Boxes: {job.BoxesCompleted}/{job.ExpectedBoxes}";

            ResultLabel.TextColor = Colors.Red;
        }
    }

    private async void OnCloseAppClicked(object sender, EventArgs e)
    {
        if (_isMismatch)
        {
            bool confirm = await DisplayAlert(
                "? Mismatch Detected",
                "This job contains discrepancies.\n\nAre you sure you want to close?",
                "Close Anyway",
                "Cancel");

            if (!confirm)
                return;
        }

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
        if (_isMismatch)
        {
            bool confirm = await DisplayAlert(
                "? Mismatch Detected",
                "You are starting a new job while this one has discrepancies.\n\nContinue?",
                "Start New Job",
                "Cancel");

            if (!confirm)
                return;
        }

        var freshJob = new PackingJob();

        Application.Current!.MainPage =
            new NavigationPage(new MainPage(freshJob));
    }
}

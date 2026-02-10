using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class VerificationPage : ContentPage
{
    PackingJob job;

    public VerificationPage(PackingJob job)
    {
        InitializeComponent();
        this.job = job;

        Verify();
    }

    void Verify()
    {
        bool totalMatch = job.PackedTotal == job.ExpectedTotal;
        bool boxMatch = job.BoxesCompleted == job.ExpectedBoxes;

        if (totalMatch && boxMatch)
        {
            ResultLabel.Text = "COUNT SUCCESSFUL\nAll items accounted for.";
            ResultLabel.TextColor = Colors.Green;
        }
        else
        {
            int difference = job.ExpectedTotal - job.PackedTotal;

            if (difference > 0)
            {
                ResultLabel.Text =
                    $"?? COUNT MISMATCH\nShort by {difference} items";
            }
            else
            {
                ResultLabel.Text =
                    $"?? COUNT MISMATCH\nOver by {Math.Abs(difference)} items";
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
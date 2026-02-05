using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class PackingPage : ContentPage
{
    private readonly PackingJob job;

    public PackingPage(PackingJob job)
    {
        InitializeComponent();
        this.job = job;
        BindingContext = job; // single source of truth
    }

    private async void OnAddItemClicked(object sender, EventArgs e)
    {
        job.CurrentBoxCount++;
        job.PackedTotal++;

        // ?? OVERPACK WARNING
        if (job.CurrentBoxCount > job.ItemsPerBox)
        {
            await DisplayAlert(
                "? Overpacked",
                "Too many items in this box!\nRemove the extra item before continuing.",
                "OK");

            job.CurrentBoxCount--;
            job.PackedTotal--;
            return;
        }

        // ? BOX COMPLETE
        if (job.CurrentBoxCount == job.ItemsPerBox)
        {
            job.BoxesCompleted++;

            await DisplayAlert(
                "Box Complete",
                "Box successfully packed.\nSeal the box and start a new one.",
                "OK");

            job.CurrentBoxCount = 0;
        }
    }

    private async void OnFinishClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VerificationPage(job));
    }
}

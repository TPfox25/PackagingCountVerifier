using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class BoxConfigPage : ContentPage
{
    private readonly PackingJob job;

    public BoxConfigPage(PackingJob job)
    {
        InitializeComponent();
        this.job = job;
        BindingContext = job;
    }

    private async void OnStartPackingClicked(object sender, EventArgs e)
    {
        // 1?? Must be greater than zero
        if (job.ItemsPerBox <= 0)
        {
            await DisplayAlert(
                "Invalid Configuration",
                "Items per box must be greater than zero.",
                "OK");
            return;
        }

        // 2?? Cannot exceed total items
        if (job.ItemsPerBox > job.ExpectedTotal)
        {
            await DisplayAlert(
                "Invalid Configuration",
                "Items per box cannot be greater than the total expected items.",
                "OK");
            return;
        }

        // 3?? (OPTIONAL but STRONGLY recommended)
        if (job.ExpectedTotal % job.ItemsPerBox != 0)
        {
            bool proceed = await DisplayAlert(
                "Uneven Packing Warning",
                $"This will result in a partially filled box.\n\n" +
                $"Expected Total: {job.ExpectedTotal}\n" +
                $"Items per Box: {job.ItemsPerBox}",
                "Proceed Anyway",
                "Fix It");

            if (!proceed)
                return;
        }

        // ? All checks passed
        await Navigation.PushAsync(new PackingPage(job));
    }
}

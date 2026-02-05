using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class MainPage : ContentPage
{
    private readonly PackingJob job;

    // ✅ Default constructor (app launch)
    public MainPage() : this(new PackingJob())
    {
    }

    // ✅ Explicit constructor (restart / resume)
    public MainPage(PackingJob job)
    {
        InitializeComponent();
        this.job = job;
        BindingContext = job;
    }

    private async void OnStartPackingClicked(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(job.JobName) ||
            string.IsNullOrWhiteSpace(job.ItemType) ||
            job.ExpectedTotal <= 0)
        {
            await DisplayAlert(
                "Missing Info",
                "Please complete all fields",
                "OK");
            return;
        }

        await Navigation.PushAsync(new BoxConfigPage(job));
    }
}

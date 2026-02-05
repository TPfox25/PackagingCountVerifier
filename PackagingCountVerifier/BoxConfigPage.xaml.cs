using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class BoxConfigPage : ContentPage
{
    PackingJob job;

    public BoxConfigPage(PackingJob job)
    {
        InitializeComponent();
        this.job = job;
        BindingContext = job;
    }

    private async void OnStartPackingClicked(object sender, EventArgs e)
    {
        if (job.ItemsPerBox <= 0)
        {
            await DisplayAlert("Error", "Items per box must be greater than 0", "OK");
            return;
        }

        await Navigation.PushAsync(new PackingPage(job));
    }
}

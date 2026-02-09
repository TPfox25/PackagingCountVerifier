using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class PackingPage : ContentPage
{
    private readonly PackingJob job;

    public PackingPage(PackingJob job)
    {
        InitializeComponent();
        this.job = job;
        BindingContext = job;
    }

    // ?? Shared logic for adding items
    private async Task AddItemsAsync(int quantity)
    {
        if (quantity <= 0)
            return;

        job.CurrentBoxCount += quantity;
        job.PackedTotal += quantity;

        // ?? OVERPACK PROTECTION
        if (job.CurrentBoxCount > job.ItemsPerBox)
        {
            await DisplayAlert(
                "? Overpacked",
                "This exceeds the box limit.\nRemove extra items before continuing.",
                "OK");

            job.CurrentBoxCount -= quantity;
            job.PackedTotal -= quantity;
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

    // ? Preset buttons (+5 / +10 / +20)
    private async void OnAddPresetClicked(object sender, EventArgs e)
    {
        if (sender is Button btn &&
            int.TryParse(btn.CommandParameter?.ToString(), out int qty))
        {
            await AddItemsAsync(qty);
        }
    }

    // ?? Manual quantity entry
    private async void OnAddManualClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(ManualQuantityEntry.Text, out int qty) || qty <= 0)
        {
            await DisplayAlert("Invalid Input", "Enter a valid quantity.", "OK");
            return;
        }

        await AddItemsAsync(qty);
        ManualQuantityEntry.Text = string.Empty;
    }

    // ? Finish packing
    private async void OnFinishClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VerificationPage(job));
    }
}

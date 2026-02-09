using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class PackingPage : ContentPage
{
    public PackingJob Job { get; }

    // Safe presets for any box size
    public List<int> Presets { get; } = new() { 1, 2, 5 };

    public PackingPage(PackingJob job)
    {
        InitializeComponent();

        Job = job;

        // ?? THIS is the correct BindingContext
        BindingContext = this;
    }

    private async Task AddItemsAsync(int quantity)
    {
        if (quantity <= 0)
            return;

        Job.CurrentBoxCount += quantity;
        Job.PackedTotal += quantity;

        // ?? OVERPACK PROTECTION
        if (Job.CurrentBoxCount > Job.ItemsPerBox)
        {
            await DisplayAlert("? Overpacked",
                               "This exceeds the box limit.",
                               "OK");

            Job.CurrentBoxCount -= quantity;
            Job.PackedTotal -= quantity;
            return;
        }

        // ? BOX COMPLETE
        if (Job.CurrentBoxCount == Job.ItemsPerBox)
        {
            Job.BoxesCompleted++;

            await DisplayAlert("Box Complete",
                               "Box packed successfully.",
                               "OK");

            Job.CurrentBoxCount = 0;
        }
    }

    private async void OnAddPresetClicked(object sender, EventArgs e)
    {
        if (sender is Button btn &&
            int.TryParse(btn.CommandParameter?.ToString(), out int qty))
        {
            await AddItemsAsync(qty);
        }
    }

    private async void OnAddManualClicked(object sender, EventArgs e)
    {
        if (!int.TryParse(ManualQuantityEntry.Text, out int qty) || qty <= 0)
        {
            await DisplayAlert("Invalid Input",
                               "Enter a valid number.",
                               "OK");
            return;
        }

        await AddItemsAsync(qty);
        ManualQuantityEntry.Text = string.Empty;
    }

    private async void OnFinishClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new VerificationPage(Job));
    }
}

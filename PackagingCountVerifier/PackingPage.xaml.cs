using PackagingCountVerifier.Models;

namespace PackagingCountVerifier;

public partial class PackingPage : ContentPage
{
    public PackingJob Job { get; }

    // Safe presets
    public List<int> Presets { get; } = new() { 1, 2, 5 };

    public PackingPage(PackingJob job)
    {
        InitializeComponent();
        Job = job;
        BindingContext = this;
    }

    private async Task AddItemsAsync(int quantity)
    {
        if (quantity <= 0)
            return;

        // ?? JOB TOTAL PROTECTION
        if (Job.PackedTotal + quantity > Job.ExpectedTotal)
        {
            await DisplayAlert(
                "Limit Reached",
                $"Only {Job.RemainingItems} items remaining in this job.",
                "OK");
            return;
        }

        // Tentatively add
        Job.CurrentBoxCount += quantity;
        Job.PackedTotal += quantity;

        // ?? BOX OVERFLOW PROTECTION
        if (Job.CurrentBoxCount > Job.ItemsPerBox)
        {
            Job.CurrentBoxCount -= quantity;
            Job.PackedTotal -= quantity;

            await DisplayAlert(
                "Overpacked Box",
                "This exceeds the box capacity.",
                "OK");
            return;
        }

        // ? BOX COMPLETE
        if (Job.CurrentBoxCount == Job.ItemsPerBox)
        {
            Job.BoxesCompleted++;

            int completedBoxNumber = Job.BoxesCompleted;

            // Reset for next box
            Job.CurrentBoxCount = 0;

            // ?? SAFETY: stop extra boxes
            if (Job.BoxesCompleted > Job.ExpectedBoxes)
                Job.BoxesCompleted = Job.ExpectedBoxes;

            // ? USER FEEDBACK (THIS IS THE KEY PART)
            await DisplayAlert(
                "? Box Completed",
                $"Box #{completedBoxNumber} packed successfully.\n\n" +
                $"Items per box: {Job.ItemsPerBox}\n" +
                $"Total packed: {Job.PackedTotal}/{Job.ExpectedTotal}",
                "OK");
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
            await DisplayAlert("Invalid Input", "Enter a valid number.", "OK");
            return;
        }

        await AddItemsAsync(qty);
        ManualQuantityEntry.Text = string.Empty;
    }

    private async void OnFinishClicked(object sender, EventArgs e)
    {
        if (!Job.IsJobComplete)
        {
            await DisplayAlert(
                "Job Incomplete",
                $"Remaining items: {Job.RemainingItems}",
                "OK");
            return;
        }

        await Navigation.PushAsync(new VerificationPage(Job));
    }
}

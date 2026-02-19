using PackagingCountVerifier.Models;
using PackagingCountVerifier.Data;
using PackagingCountVerifier.Data.Entities;

namespace PackagingCountVerifier;

public partial class PackingPage : ContentPage
{
    public PackingJob Job { get; }

    private readonly AppDatabase _db;

    public List<int> Presets { get; } = new() { 1, 2, 5 };

    public PackingPage(PackingJob job)
    {
        InitializeComponent();

        Job = job;

        _db = Application.Current!.Handler
                .MauiContext!
                .Services
                .GetService<AppDatabase>()!;

        BindingContext = this;
    }

    private async Task ShowBoxCompletedBanner(int boxNumber)
    {
        BoxCompletedLabel.Text =
            $"? Box #{boxNumber} completed • " +
            $"{Job.ItemsPerBox} items • " +
            $"{Job.PackedTotal}/{Job.ExpectedTotal} packed";

        BoxCompletedBanner.IsVisible = true;

        await Task.Delay(3000);

        BoxCompletedBanner.IsVisible = false;
    }

    private async Task AddItemsAsync(int quantity)
    {
        if (quantity <= 0)
            return;

        // ?? BOX OVERFLOW PROTECTION ONLY
        if (Job.CurrentBoxCount + quantity > Job.ItemsPerBox)
        {
            await DisplayAlert(
                "Overpacked Box",
                "This exceeds the box capacity.",
                "OK");
            return;
        }

        // Add items
        Job.CurrentBoxCount += quantity;
        Job.PackedTotal += quantity;

        // ? BOX COMPLETE
        if (Job.CurrentBoxCount == Job.ItemsPerBox)
        {
            Job.BoxesCompleted++;
            int completedBoxNumber = Job.BoxesCompleted;

            Job.CurrentBoxCount = 0;

            // ?? SAVE TO DATABASE
            await _db.InsertBoxAsync(new BoxHistoryEntity
            {
                PackingJobId = Job.Id,
                BoxNumber = completedBoxNumber,
                ItemsInBox = Job.ItemsPerBox,
                PackedAt = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")
            });

            await ShowBoxCompletedBanner(completedBoxNumber);
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
        bool totalMatch = Job.PackedTotal == Job.ExpectedTotal;
        bool boxMatch = Job.BoxesCompleted == Job.ExpectedBoxes;

        // ?? TRUE INCOMPLETE (still items missing)
        if (Job.PackedTotal < Job.ExpectedTotal)
        {
            await DisplayAlert(
                "Job Incomplete",
                $"Remaining items: {Job.ExpectedTotal - Job.PackedTotal}",
                "OK");

            return;
        }

        // ? MISMATCH WARNING
        if (!totalMatch || !boxMatch)
        {
            bool proceed = await DisplayAlert(
                "? Verification Warning",
                $"Totals do not match.\n\n" +
                $"Packed: {Job.PackedTotal}/{Job.ExpectedTotal}\n" +
                $"Boxes: {Job.BoxesCompleted}/{Job.ExpectedBoxes}\n\n" +
                "Continue to verification?",
                "Continue",
                "Cancel");

            if (!proceed)
                return;
        }

        // ? Go to verification regardless
        await Navigation.PushAsync(
            new VerificationPage(Job, _db));
    }

}

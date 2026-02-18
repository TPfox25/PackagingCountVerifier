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

        // ?? JOB TOTAL PROTECTION
        if (Job.PackedTotal + quantity > Job.ExpectedTotal)
        {
            await DisplayAlert(
                "Limit Reached",
                $"Only {Job.RemainingItems} items remaining.",
                "OK");
            return;
        }

        // Tentative add
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

            Job.CurrentBoxCount = 0;

            if (Job.BoxesCompleted > Job.ExpectedBoxes)
                Job.BoxesCompleted = Job.ExpectedBoxes;

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
        if (!Job.IsJobComplete)
        {
            await DisplayAlert(
                "Job Incomplete",
                $"Remaining items: {Job.RemainingItems}",
                "OK");
            return;
        }

        await Navigation.PushAsync(
            new VerificationPage(Job, _db));
    }
}

using BusModule;
using System.Windows;
using System.Windows.Controls;

namespace WpfJLPT;

public partial class ReviewPage : Page
{
    private readonly JlptCat _categoryService = new();
    private bool _isLoadingCategories;

    public ReviewPage()
    {
        InitializeComponent();
        Loaded += ReviewPage_Loaded;
        ReviewItemsComboBox.SelectionChanged += ReviewItemsComboBox_SelectionChanged;
    }

    private async void ReviewPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ReviewItemsComboBox.DisplayMemberPath = nameof(JlptCategory.Name);
            ReviewItemsComboBox.SelectedValuePath = nameof(JlptCategory.Id);
            _isLoadingCategories = true;
            ReviewItemsComboBox.ItemsSource = await _categoryService.GetCategoriesAsync();
            ReviewItemsComboBox.SelectedValue = 7;

            if (ReviewItemsComboBox.SelectedValue is int categoryId)
            {
                await LoadReviewEntryAsync(categoryId);
            }
            else
            {
                ReviewItemTextBlock.Text = string.Empty;
                ReviewTextDisplay.Clear();
            }
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Could not load JLPT categories: {exception.Message}",
                "Category Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            _isLoadingCategories = false;
        }
    }

    private async void ReviewItemsComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (_isLoadingCategories || ReviewItemsComboBox.SelectedValue is not int categoryId)
        {
            return;
        }

        await LoadReviewEntryAsync(categoryId);
    }

    private async Task LoadReviewEntryAsync(int categoryId)
    {
        try
        {
            var entries = await _categoryService.GetEntriesAsync(categoryId);
            if (entries.Count == 0)
            {
                ReviewItemTextBlock.Text = string.Empty;
                ReviewTextDisplay.Text = "No JLPT entries found for this category.";
                return;
            }

            var entry = entries[0];
            ReviewItemTextBlock.Text = entry.Entry;
            ReviewTextDisplay.Text =
                $"Entry: {entry.Entry}{Environment.NewLine}" +
                $"Meaning: {entry.Meaning}{Environment.NewLine}" +
                $"Hiragana: {entry.Hiragana}{Environment.NewLine}" +
                $"Katakana: {entry.Katakana}{Environment.NewLine}" +
                $"Positive: {entry.Positive}{Environment.NewLine}" +
                $"Negative: {entry.Negative}{Environment.NewLine}" +
                $"Past Positive: {entry.PastPositive}{Environment.NewLine}" +
                $"Past Negative: {entry.PastNegative}";
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Could not load JLPT entries: {exception.Message}",
                "Entry Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

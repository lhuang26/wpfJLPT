using BusModule;
using System.Windows;
using System.Windows.Controls;

namespace WpfJLPT;

public partial class ReviewPage : Page
{
    private readonly JlptCat _categoryService = new();
    private bool _isLoadingCategories;
    private JlptEntry? _selectedEntry;

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
            var categories = await _categoryService.GetCategoriesAsync();
            ReviewItemsComboBox.ItemsSource = categories;
            var defaultCategory = categories.FirstOrDefault(category => category.Id == 7);
            if (defaultCategory is not null)
            {
                ReviewItemsComboBox.SelectedItem = defaultCategory;
                await LoadReviewEntryAsync(defaultCategory.Id);
            }
            else
            {
                ReviewItemTextBlock.Text = string.Empty;
                ReviewDetailsGrid.Visibility = Visibility.Collapsed;
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

        _selectedEntry = null;
        ReviewDetailsGrid.Visibility = Visibility.Collapsed;
        await LoadReviewEntryAsync(categoryId);
    }

    private void ViewButton_Click(object sender, RoutedEventArgs e)
    {
        if (_selectedEntry is not { } entry)
        {
            ReviewDetailsGrid.Visibility = Visibility.Collapsed;
            return;
        }

        EntryValueTextBlock.Text = entry.Entry;
        MeaningValueTextBlock.Text = entry.Meaning;
        HiraganaValueTextBlock.Text = entry.Hiragana;
        KatakanaValueTextBlock.Text = entry.Katakana;
        PositiveValueTextBlock.Text = entry.Positive;
        NegativeValueTextBlock.Text = entry.Negative;
        PastPositiveValueTextBlock.Text = entry.PastPositive;
        PastNegativeValueTextBlock.Text = entry.PastNegative;
        ReviewDetailsGrid.Visibility = Visibility.Visible;
    }

    private async void PassButton_Click(object sender, RoutedEventArgs e)
    {
        await OrderAndLoadEntryAsync(1);
    }

    private async void RefreshButton_Click(object sender, RoutedEventArgs e)
    {
        await OrderAndLoadEntryAsync(-1);
    }

    private async Task OrderAndLoadEntryAsync(int orderType)
    {
        if (_selectedEntry is not { } entry ||
            ReviewItemsComboBox.SelectedValue is not int categoryId)
        {
            return;
        }

        PassButton.IsEnabled = false;
        RefreshButton.IsEnabled = false;
        ReviewDetailsGrid.Visibility = Visibility.Collapsed;

        try
        {
            await _categoryService.OrderEntryAsync(entry.Id, orderType);
            await LoadReviewEntryAsync(categoryId, entry.Id);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Could not update the JLPT review order: {exception.Message}",
                "Review Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            PassButton.IsEnabled = true;
            RefreshButton.IsEnabled = true;
        }
    }

    private async Task LoadReviewEntryAsync(int categoryId, int? previousEntryId = null)
    {
        try
        {
            var entries = await _categoryService.GetEntriesAsync(categoryId);
            if (entries.Count == 0)
            {
                _selectedEntry = null;
                ReviewItemTextBlock.Text = string.Empty;
                ReviewDetailsGrid.Visibility = Visibility.Collapsed;
                return;
            }

            var nextIndex = 0;
            if (previousEntryId is int entryId)
            {
                for (var index = 0; index < entries.Count; index++)
                {
                    if (entries[index].Id == entryId)
                    {
                        nextIndex = (index + 1) % entries.Count;
                        break;
                    }
                }
            }

            _selectedEntry = entries[nextIndex];
            ReviewItemTextBlock.Text = _selectedEntry.Entry;
            ReviewDetailsGrid.Visibility = Visibility.Collapsed;
        }
        catch (Exception exception)
        {
            _selectedEntry = null;
            ReviewItemTextBlock.Text = string.Empty;
            ReviewDetailsGrid.Visibility = Visibility.Collapsed;
            MessageBox.Show(
                $"Could not load JLPT entries: {exception.Message}",
                "Entry Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

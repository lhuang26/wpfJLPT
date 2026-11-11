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
                ReviewTextDisplay.Clear();
                return;
            }

            var entry = entries[0];
            ReviewItemTextBlock.Text = entry.Entry;
            ReviewTextDisplay.Clear();
        }
        catch (Exception exception)
        {
            ReviewItemTextBlock.Text = string.Empty;
            MessageBox.Show(
                $"Could not load JLPT entries: {exception.Message}",
                "Entry Load Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
    }
}

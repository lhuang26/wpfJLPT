using BusModule;
using System.Windows;
using System.Windows.Controls;

namespace WpfJLPT;

public partial class DataEntryPage : Page
{
    private readonly JlptCat _categoryService = new();

    public DataEntryPage()
    {
        InitializeComponent();
        Loaded += DataEntryPage_Loaded;
    }

    private async void DataEntryPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            var categories = await _categoryService.GetCategoriesAsync();
            CategoryCheckBoxPanel.Children.Clear();

            for (var index = 0; index < categories.Count; index++)
            {
                CategoryCheckBoxPanel.Children.Add(new CheckBox
                {
                    Content = categories[index].Name,
                    Tag = categories[index].Id,
                    Margin = index < categories.Count - 1
                        ? new Thickness(0, 0, 16, 0)
                        : new Thickness(0)
                });
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
    }

    private async void SubmitButton_Click(object sender, RoutedEventArgs e)
    {
        SubmitButton.IsEnabled = false;

        try
        {
            var categoryIds = CategoryCheckBoxPanel.Children
                .OfType<CheckBox>()
                .Where(checkBox => checkBox.IsChecked == true)
                .Select(checkBox => checkBox.Tag)
                .OfType<int>()
                .ToArray();

            var entry = new JlptEntryData(
                EntryTextBox.Text,
                MeaningTextBox.Text,
                HiraganaTextBox.Text,
                KatakanaTextBox.Text,
                PositiveTextBox.Text,
                NegativeTextBox.Text,
                PastTextBox.Text,
                PastNegTextBox.Text,
                categoryIds);

            await _categoryService.SaveEntryAsync(entry);

            MessageBox.Show(
                "Entry saved successfully.",
                "Data Entry",
                MessageBoxButton.OK,
                MessageBoxImage.Information);
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Could not save the entry: {exception.Message}",
                "Save Error",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
        }
        finally
        {
            SubmitButton.IsEnabled = true;
        }
    }
}

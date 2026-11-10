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
                    Content = categories[index],
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
}

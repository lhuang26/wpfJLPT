using BusModule;
using System.Windows;
using System.Windows.Controls;

namespace WpfJLPT;

public partial class ReviewPage : Page
{
    private readonly JlptCat _categoryService = new();

    public ReviewPage()
    {
        InitializeComponent();
        Loaded += ReviewPage_Loaded;
    }

    private async void ReviewPage_Loaded(object sender, RoutedEventArgs e)
    {
        try
        {
            ReviewItemsComboBox.ItemsSource = await _categoryService.GetCategoriesAsync();
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

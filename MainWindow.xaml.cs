using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace WpfJLPT;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        PageFrame.Navigate(new ReviewPage());
    }

    private void Review_Click(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new ReviewPage());
    }

    private void DataEntry_Click(object sender, RoutedEventArgs e)
    {
        PageFrame.Navigate(new DataEntryPage());
    }
}
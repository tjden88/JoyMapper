using System.Windows;
using JoyMapper.ViewModels;

namespace JoyMapper.Views;

/// <summary>
/// Interaction logic for MainWindow.xaml
/// </summary>
public partial class MainWindow : Window
{
    public MainWindowViewModel ViewModel { get; set; }

    public MainWindow(MainWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
        DataContext = ViewModel;
    }

    private void MainWindow_OnLoaded(object Sender, RoutedEventArgs E)
    {
        ViewModel.LoadDataCommand.Execute();
        ViewModel.CheckUpdatesCommand.Execute();
    }
}
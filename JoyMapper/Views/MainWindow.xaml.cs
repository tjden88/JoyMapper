using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
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

    private void PatternsList_OnMouseDoubleClick(object Sender, MouseButtonEventArgs E)
    {
        if (E.ClickCount == 2)
            Debug.WriteLine("dc");
    }
}
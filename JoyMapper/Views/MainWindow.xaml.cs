using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using JoyMapper.Models;
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
        UpdatePatternsView(null, new RoutedEventArgs());
    }


    private void UpdatePatternsView(object sender, RoutedEventArgs e)
    {
        var patternsListItemsSource = PatternsList?.ItemsSource;
        if (patternsListItemsSource == null)
            return;

        var source = CollectionViewSource.GetDefaultView(patternsListItemsSource);
        if(source == null)
            return;

        source.SortDescriptions.Clear();
        source.GroupDescriptions.Clear();

        if (PatternsGroupsToggle.IsChecked == true)
        {
            source.GroupDescriptions.Add(new PropertyGroupDescription(nameof(JoyPattern.GroupName)));
            source.SortDescriptions.Add(new(nameof(JoyPattern.GroupName), ListSortDirection.Ascending));
        }

        var item = PatternsListSortByCombo.SelectedItem as ComboBoxItem;
        var orderBy = item?.Tag?.ToString();
        if (orderBy != null)
            source.SortDescriptions.Add(new(orderBy, ListSortDirection.Ascending));
    }
}
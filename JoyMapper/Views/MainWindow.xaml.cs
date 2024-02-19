using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media.Animation;
using System.Windows.Threading;
using JoyMapper.Models;
using JoyMapper.ViewModels;
using WPR.Animations;

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
        if (source == null)
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


    private void ActiveProfileStateChanged(object Sender, DependencyPropertyChangedEventArgs E)
    {
        if (ClipEllipce == null) return;

        ClipEllipce.Center = Mouse.GetPosition(RootGrid);

        if (ViewModel.IsProfileStarted)
        {
            AnimateIndicator();
        }
        else
            AnimateIndicatorBack();

    }

    private void AnimateIndicator()
    {
        var sideY = ActualHeight - ClipEllipce.Center.Y > ClipEllipce.Center.Y
            ? ActualHeight
            : 0;
        var hypo = (ClipEllipce.Center - new Point(0, sideY)).Length;
        const double durationMs = 400d;

        new Storyboard()
            .AddDoubleAnimation("Clip.RadiusX", 0, hypo, durationMs, EasingFunctions.CircleEaseOut)
            .ClearOnComplete()
            .OnComplete(() => ClipEllipce.RadiusX = 10000)
            .Begin(ActiveProfileControl)
            ;

        new Storyboard()
            .AddDoubleAnimation("Opacity", 1, 0, durationMs)
            .Begin(BgBorder)
            ;
    }

    private void AnimateIndicatorBack()
    {
        const double durationMs = 400d;

        new Storyboard()
            .AddDoubleAnimation("Clip.RadiusX", Math.Max(ActualWidth, ActualHeight), 0, durationMs, EasingFunctions.CircleEaseIn)
            .Begin(ActiveProfileControl)
            ;

        new Storyboard()
            .AddDoubleAnimation("Opacity", 0, 1, durationMs)
            .Begin(BgBorder)
            ;
    }
}
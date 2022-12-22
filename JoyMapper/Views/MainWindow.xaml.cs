using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Threading;
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
    // https://stackoverflow.com/questions/31120492/how-to-create-beginstoryboard-in-code-behind-for-wpf

    private void AnimateIndicator()
    {
        var duration = TimeSpan.FromSeconds(0.4);
        var ease = new CircleEase { EasingMode = EasingMode.EaseOut };
        var sideY = ActualHeight - ClipEllipce.Center.Y > ClipEllipce.Center.Y
            ? ActualHeight
            : 0;

        var hypo = (ClipEllipce.Center - new Point(0, sideY)).Length;

        var sizeAnimation = new DoubleAnimation(0, hypo, duration) { EasingFunction = ease };
        var opacityAnimation = new DoubleAnimation(1, 0, duration);
        var storyboard = new Storyboard
        {
            Children = new()
            {
                sizeAnimation,
            }
        };

        Storyboard.SetTarget(storyboard, ActiveProfileControl);

        Storyboard.SetTargetProperty(sizeAnimation, new PropertyPath("Clip.RadiusX"));
        storyboard.Completed += (_, _) =>
        {
            ClipEllipce.BeginAnimation(EllipseGeometry.RadiusXProperty, null);
            ClipEllipce.RadiusX = 10000;
        };

        Application.Current.Dispatcher.Invoke(DispatcherPriority.ApplicationIdle, () =>
        {
            storyboard.Begin(ActiveProfileControl);
            BgBorder.BeginAnimation(OpacityProperty, opacityAnimation);
        });
    }

    private void AnimateIndicatorBack()
    {
        var duration = TimeSpan.FromSeconds(0.4);
        var ease = new CircleEase { EasingMode = EasingMode.EaseIn };

        var sizeAnimation = new DoubleAnimation(Math.Max(ActualWidth, ActualHeight), 0, duration) { EasingFunction = ease };
        var opacityAnimation = new DoubleAnimation(0, 1, duration);
        var storyboard = new Storyboard
        {
            Children = new()
            {
                sizeAnimation,
            }
        };

        Storyboard.SetTarget(storyboard, ActiveProfileControl);

        Storyboard.SetTargetProperty(sizeAnimation, new PropertyPath("Clip.RadiusX"));

        storyboard.Completed += (_, _) =>
        {
            ClipEllipce.BeginAnimation(EllipseGeometry.RadiusXProperty, null);
            ClipEllipce.RadiusX = 0;
        };

        storyboard.Begin(ActiveProfileControl);
        BgBorder.BeginAnimation(OpacityProperty, opacityAnimation);

    }

}
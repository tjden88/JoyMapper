using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
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

    private void Ellips_OnIsEnabledChanged(object Sender, DependencyPropertyChangedEventArgs E)
    {
        if(!IsActive) return;

        var position = Mouse.GetPosition(RootGrid);
        var size = Math.Max(ActualHeight, ActualWidth);

        AnimationEllipse.Width = size;

        Canvas.SetLeft(AnimationEllipse, position.X - size / 2);
        Canvas.SetTop(AnimationEllipse, position.Y - size / 2);
        if (AnimationEllipse.IsEnabled)
            AnimateEllipse();
        else
            AnimateEllipseBack();
    }
    // https://stackoverflow.com/questions/31120492/how-to-create-beginstoryboard-in-code-behind-for-wpf

    private void AnimateEllipse()
    {
        var duration = TimeSpan.FromSeconds(0.6);
        var ease = new CircleEase {EasingMode = EasingMode.EaseOut};
        var sizeAnimation = new DoubleAnimation(0, 2, duration) {EasingFunction = ease};
        var opacityAnimation = new DoubleAnimation(1, 0, duration) {EasingFunction = ease, BeginTime = TimeSpan.FromSeconds(0.2)};
        var storyboard = new Storyboard
        {
            Children = new()
            {
                sizeAnimation,
                opacityAnimation,
            }
        };

        Storyboard.SetTarget(storyboard, AnimationEllipse);

        Storyboard.SetTargetProperty(sizeAnimation, new PropertyPath("RenderTransform.ScaleX"));
        Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));


        storyboard.Begin(AnimationEllipse);
        
    }
    private void AnimateEllipseBack()
    {
        var duration = TimeSpan.FromSeconds(0.6);
        var ease = new CircleEase { EasingMode = EasingMode.EaseOut };
        var sizeAnimation = new DoubleAnimation(2, 0, duration) { EasingFunction = ease };
        var opacityAnimation = new DoubleAnimation(0, 1, duration) { EasingFunction = ease };
        var storyboard = new Storyboard
        {
            Children = new()
            {
                sizeAnimation,
                opacityAnimation,
            }
        };

        Storyboard.SetTarget(storyboard, AnimationEllipse);

        Storyboard.SetTargetProperty(sizeAnimation, new PropertyPath("RenderTransform.ScaleX"));
        Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));


        storyboard.Begin(AnimationEllipse);

    }
}
using System.Windows;
using JoyMapper.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Views
{
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

        private void ButtonBase_OnClick(object Sender, RoutedEventArgs E)
        {
            var wnd = App.Services.GetRequiredService<Windows.EditPatternWindow>();
            wnd.Owner = this;
            wnd.ShowDialog();
        }
    }
}

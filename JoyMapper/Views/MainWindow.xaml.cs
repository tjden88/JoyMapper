using System.Windows;
using JoyMapper.ViewModels;
using JoyMapper.Views.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        internal MainWindowViewModel ViewModel { get; set; } = new();

        public MainWindow()
        {
            InitializeComponent();
            DataContext = ViewModel;
        }

        private void ButtonBase_OnClick(object Sender, RoutedEventArgs E)
        {
            var wnd = App.Services.GetRequiredService<AddJoyBinding>();
            wnd.Owner = this;
            wnd.ShowDialog();
        }
    }
}

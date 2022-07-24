using System.Windows;

namespace JoyMapper
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

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var wnd = new AddPattern();
            wnd.Show();
        }
    }
}

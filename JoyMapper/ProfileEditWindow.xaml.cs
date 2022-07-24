using System.Windows;

namespace JoyMapper
{
    /// <summary>
    /// Логика взаимодействия для ProfileEditWindow.xaml
    /// </summary>
    public partial class ProfileEditWindow : Window
    {
        internal ProfileEditWindowViewModel ProfileEditWindowViewModel = new();
        public ProfileEditWindow()
        {
            DataContext = ProfileEditWindowViewModel;
            InitializeComponent();
        }
    }
}

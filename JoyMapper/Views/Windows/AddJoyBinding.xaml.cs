using System.Windows;
using JoyMapper.ViewModels.Windows;

namespace JoyMapper.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddJoyBinding.xaml
    /// </summary>
    public partial class AddJoyBinding : Window
    {
        public AddJoyBinding(AddJoyBindingViewModel viewModel)
        {
            ViewModel = viewModel;
            InitializeComponent();
        }

        public AddJoyBindingViewModel ViewModel { get; set; }



    }
}

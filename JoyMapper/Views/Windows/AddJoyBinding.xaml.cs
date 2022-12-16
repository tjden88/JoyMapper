using System.Windows;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для AddJoyBinding.xaml
    /// </summary>
    public partial class AddJoyBinding : Window
    {
        public AddJoyBinding(AddJoyBindingViewModel viewModel)
        {
            InitializeComponent();
            ViewModel = viewModel;
        }

        public AddJoyBindingViewModel ViewModel { get; set; }


        public class AddJoyBindingViewModel
        {

        }
    }
}

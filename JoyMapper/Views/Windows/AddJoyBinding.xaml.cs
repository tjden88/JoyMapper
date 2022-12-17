using System;
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

        public AddJoyBindingViewModel ViewModel { get; }


        private void AddJoyBinding_OnLoaded(object Sender, RoutedEventArgs E)
        {
            ViewModel.StartWatching();
        }

        private void AddJoyBinding_OnClosed(object Sender, EventArgs E)
        {
            ViewModel.StopWatching();
        }

        private void ButtonAccept_OnClick(object Sender, RoutedEventArgs E)
        {
            DialogResult = true;
        }
    }
}

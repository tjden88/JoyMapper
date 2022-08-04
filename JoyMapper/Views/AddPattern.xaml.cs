using System.Windows;
using System.Windows.Input;
using JoyMapper.Models;
using JoyMapper.ViewModels;

namespace JoyMapper.Views
{
    /// <summary>
    /// Логика взаимодействия для AddPattern.xaml
    /// </summary>
    public partial class AddPattern : Window
    {
        public AddPattern()
        {
            InitializeComponent();
        }

        private AddPatternViewModel ViewModel => (AddPatternViewModel)DataContext;

        private void AddPattern_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = ViewModel;
            if (vm.JoyAction?.IsRecording != true) return;

            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            vm.JoyAction.AddKeyBinding(key, true);
            e.Handled = true;
        }


        private void AddPattern_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var vm = ViewModel;
            if (vm.JoyAction?.IsRecording != true) return;

            var key = e.Key == Key.System ? e.SystemKey : e.Key;

            vm.JoyAction.AddKeyBinding(key, false);
            e.Handled = true;
        }

    }
}

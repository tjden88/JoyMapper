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
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (vm.IsPressRecorded && !e.IsRepeat)
            {
                vm.PressKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = true,
                    KeyCode = key,
                });
                e.Handled = true;
            }

            if (vm.IsReleaseRecorded && !e.IsRepeat)
            {
                vm.ReleaseKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = true,
                    KeyCode = key
                });
                e.Handled = true;
            }
        }


        private void AddPattern_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var vm = ViewModel;
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (vm.IsPressRecorded)
            {
                vm.PressKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = false,
                    KeyCode = key
                });
                e.Handled = true;
            }

            if (vm.IsReleaseRecorded)
            {
                vm.ReleaseKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = false,
                    KeyCode = key
                });
                e.Handled = true;
            }
        }

    }
}

using System.Windows;
using System.Windows.Input;
using JoyMapper.ViewModels;

namespace JoyMapper.Views
{
    /// <summary>
    /// Логика взаимодействия для EditPatternWindow.xaml
    /// </summary>
    public partial class EditPatternWindow : Window
    {
        public EditPatternWindow()
        {
            InitializeComponent();
        }

        private EditPatternWindowViewModel WindowViewModel => (EditPatternWindowViewModel)DataContext;

        private void AddPattern_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = WindowViewModel;
            if (vm.JoyAction?.IsRecording != true) return;
            if (!e.IsRepeat)
            {
                var key = e.Key == Key.System ? e.SystemKey : e.Key;

                vm.JoyAction.AddKeyBinding(key, true);

            }
            e.Handled = true;
        }


        private void AddPattern_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var vm = WindowViewModel;
            if (vm.JoyAction?.IsRecording != true) return;
            if (!e.IsRepeat)
            {
                var key = e.Key == Key.System ? e.SystemKey : e.Key;

                vm.JoyAction.AddKeyBinding(key, false);

            }
            e.Handled = true;
        }

    }
}

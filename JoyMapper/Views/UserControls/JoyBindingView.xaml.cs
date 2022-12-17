using System.Diagnostics;
using System.Windows.Controls;
using JoyMapper.ViewModels.UserControls;

namespace JoyMapper.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для JoyBindingView.xaml
    /// </summary>
    public partial class JoyBindingView : UserControl
    {
        public JoyBindingView()
        {
            InitializeComponent();
        }

        private void OnUnloaded(object sender, System.Windows.RoutedEventArgs e)
        {
            if (DataContext is not JoyBindingViewModel { } vm)
                return;

            vm.Dispose();
            Debug.WriteLine("Вьюмодель отслеживания кнопки уничтожена");
        }
    }
}

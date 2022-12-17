using System.Windows;
using JoyMapper.ViewModels.Windows;

namespace JoyMapper.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditPatternWindow.xaml
    /// </summary>
    public partial class EditPatternWindow : Window
    {
        public EditPatternViewModel ViewModel { get; }

        public EditPatternWindow(EditPatternViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            InitializeComponent();
        }


    }
}

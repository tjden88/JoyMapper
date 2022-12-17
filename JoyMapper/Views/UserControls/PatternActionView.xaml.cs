using System.Windows.Controls;
using JoyMapper.ViewModels.UserControls;

namespace JoyMapper.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для PatternActionView.xaml
    /// </summary>
    public partial class PatternActionView : UserControl
    {
        public PatternActionViewModel ViewModel { get; }

        public PatternActionView(PatternActionViewModel patternActionViewModel)
        {
            ViewModel = patternActionViewModel;
            InitializeComponent();
        }
    }
}

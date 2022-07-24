using System.Windows;
using System.Windows.Input;

namespace JoyMapper
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


        #region IsPressRecorded : bool - Нажата ли кнопка записи при нажатии

        /// <summary>Нажата ли кнопка записи при нажатии</summary>
        private bool _IsPressRecorded;

        /// <summary>Нажата ли кнопка записи при нажатии</summary>
        public bool IsPressRecorded
        {
            get => _IsPressRecorded;
            set
            {
                if (Equals(value, _IsPressRecorded)) return;
                _IsPressRecorded = value;
                RecordPressButton.Content = value
                    ? "Остановить запись"
                    : "Начать запись";
            }
        }

        #endregion

        

        private void ButtonPressRecord_Click(object sender, RoutedEventArgs e)
        {
            IsPressRecorded = !IsPressRecorded;
        }

        private void AddPattern_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsPressRecorded)
                MessageBox.Show(e.Key.ToString());
        }
    }
}

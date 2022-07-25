using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Models;
using WPR.MVVM.Commands;

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


        #region IsReleaseRecorded : bool - Нажата ли кнопка записи при отпускании

        /// <summary>Нажата ли кнопка записи при отпускании</summary>
        private bool _IsReleaseRecorded;

        /// <summary>Нажата ли кнопка записи при отпускании</summary>
        public bool IsReleaseRecorded
        {
            get => _IsReleaseRecorded;
            set
            {
                if (Equals(value, _IsReleaseRecorded)) return;
                _IsReleaseRecorded = value;
                RecordReleaseButton.Content = value
                    ? "Остановить запись"
                    : "Начать запись";
            }
        }

        #endregion


        #region JoyName : string - Имя джойстика

        /// <summary>Имя джойстика</summary>
        private string _JoyName;

        /// <summary>Имя джойстика</summary>
        public string JoyName
        {
            get => _JoyName;
            set
            {
                if (Equals(value, _JoyName)) return;
                _JoyName = value;
                JoyNameText.Text = value;
            }
        }

        #endregion


        #region JoyButton : int - Выбранная кнопка

        /// <summary>Выбранная кнопка</summary>
        private int _JoyButton;

        /// <summary>Выбранная кнопка</summary>
        public int JoyButton
        {
            get => _JoyButton;
            set
            {
                if (Equals(value, _JoyButton)) return;
                _JoyButton = value;
                JoyBtnNumberText.Text = "Кнопка " + value;
            }
        }

        #endregion


        #region Имя паттерна

        /// <summary> Имя паттерна </summary>
        public string PatternName
        {
            get => PatternNameText.Text.Trim();
            set => PatternNameText.Text = value;
        } 
        #endregion


        #region Type : ObservableCollection<KeyboardKeyBinding> - Кнопки клавиатуры при нажатии

        /// <summary>Кнопки клавиатуры при нажатии</summary>
        private ObservableCollection<KeyboardKeyBinding> _PressKeyBindings = new();

        /// <summary>Кнопки клавиатуры при нажатии</summary>
        public ObservableCollection<KeyboardKeyBinding> PressKeyBindings
        {
            get => _PressKeyBindings;
            set
            {
                if (Equals(value, _PressKeyBindings)) return;
                _PressKeyBindings = value;
                PressKeyBindingsList.ItemsSource = value;
            }
        }

        #endregion


        #region Type : ObservableCollection<KeyboardKeyBinding> - Кнопки клавиатуры при отпускании

        /// <summary>Кнопки клавиатуры при отпускании</summary>
        private ObservableCollection<KeyboardKeyBinding> _ReleaseKeyBindings = new();

        /// <summary>Кнопки клавиатуры при отпускании</summary>
        public ObservableCollection<KeyboardKeyBinding> ReleaseKeyBindings
        {
            get => _ReleaseKeyBindings;
            set
            {
                if (Equals(value, _ReleaseKeyBindings)) return;
                _ReleaseKeyBindings = value;
                ReleaseKeyBindingsList.ItemsSource = value;
            }
        }

        #endregion


        private void ButtonPressRecord_Click(object sender, RoutedEventArgs e)
        {
            IsPressRecorded = !IsPressRecorded;
        }
        private void ButtonReleaseRecord_Click(object sender, RoutedEventArgs e)
        {
            IsReleaseRecorded = !IsReleaseRecorded;
        }

        private void AddPattern_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (IsPressRecorded && !e.IsRepeat)
                PressKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = true,
                    KeyCode = e.Key
                });

            if (IsReleaseRecorded && !e.IsRepeat)
                ReleaseKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = true,
                    KeyCode = e.Key
                });
        }


        private void AddPattern_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            if (IsPressRecorded)
                PressKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = false,
                    KeyCode = e.Key
                });

            if (IsReleaseRecorded)
                ReleaseKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = false,
                    KeyCode = e.Key
                });
        }

        private void ButtonClearPress_Click(object sender, RoutedEventArgs e)
        {
            IsPressRecorded = false;
            PressKeyBindings.Clear();
        }

        private void ButtonClearRelease_Click(object sender, RoutedEventArgs e)
        {
            IsReleaseRecorded = false;
            ReleaseKeyBindings.Clear();
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (JoyName == null || JoyButton < 0)
            {
                MessageBox.Show("Не определена кнопка контроллера для назначения паттерна");
                return;
            }

            if (PressKeyBindings.Count == 0 && ReleaseKeyBindings.Count == 0)
            {
                MessageBox.Show("Клавиатурные команды не назначены");
                return;
            }

            if (string.IsNullOrEmpty(PatternName))
            {
                MessageBox.Show("Введите имя паттерна");
                return;
            }

            DialogResult = true;
        }

        #region Command AttachJoyButtonCommand - Определить кнопку джойстика

        /// <summary>Определить кнопку джойстика</summary>
        private Command _AttachJoyButtonCommand;

        /// <summary>Определить кнопку джойстика</summary>
        public Command AttachJoyButtonCommand => _AttachJoyButtonCommand
            ??= new Command(OnAttachJoyButtonCommandExecuted, CanAttachJoyButtonCommandExecute, "Определить кнопку джойстика");

        /// <summary>Проверка возможности выполнения - Определить кнопку джойстика</summary>
        private bool CanAttachJoyButtonCommandExecute() => true;

        /// <summary>Логика выполнения - Определить кнопку джойстика</summary>
        private void OnAttachJoyButtonCommandExecuted()
        {
            var wnd = new AddJoyButton { Owner = this };
            var result = wnd.ShowDialog();
            if(result != true) return;
            JoyButton = wnd.JoyKey;
            JoyName = wnd.JoyName;

        }

        #endregion
    }
}

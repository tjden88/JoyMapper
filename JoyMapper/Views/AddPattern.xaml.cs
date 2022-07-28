using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JoyMapper.Models;
using WPR.MVVM.Commands;

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

        private void AddPattern_OnPreviewKeyDown(object sender, KeyEventArgs e)
        {

            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (IsPressRecorded && !e.IsRepeat)
                PressKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = true,
                    KeyCode = key,
                });

            if (IsReleaseRecorded && !e.IsRepeat)
                ReleaseKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = true,
                    KeyCode = key
                });
        }


        private void AddPattern_OnPreviewKeyUp(object sender, KeyEventArgs e)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            if (IsPressRecorded)
                PressKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = false,
                    KeyCode = key
                });

            if (IsReleaseRecorded)
                ReleaseKeyBindings.Add(new KeyboardKeyBinding()
                {
                    IsPress = false,
                    KeyCode = key
                });
        }

        private void ButtonSave_Click(object sender, RoutedEventArgs e)
        {
            if (JoyName == null || JoyButton < 1)
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

    }
}

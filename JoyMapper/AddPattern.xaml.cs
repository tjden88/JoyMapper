﻿using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Models;

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




        public ObservableCollection<KeyboardKeyBinding> PressKeyBindings { get; set; } = new();
        public ObservableCollection<KeyboardKeyBinding> ReleaseKeyBindings { get; set; } = new();


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

    }
}

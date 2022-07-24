using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Models;
using SharpDX.DirectInput;
using WPR.MVVM.Commands;

namespace JoyMapper
{
    /// <summary>
    /// Логика взаимодействия для AddJoyButton.xaml
    /// </summary>
    public partial class AddJoyButton : Window
    {
        public AddJoyButton()
        {
            InitializeComponent();
            LoadJoyDevices();
            ListenPressButton();
        }


        private string JoyName { get; set; }
        private int JoyKey { get; set; }

        private bool _Accepted;


        /// <summary>Список подключённых контроллеров</summary>
        private List<DeviceInstance> _JoyDevices;

        private void LoadJoyDevices()
        {
            var devices = new DirectInput().GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);
            _JoyDevices = devices.ToList();
        }


        private async void ListenPressButton()
        {
            var joys = _JoyDevices
                .Select(j => new Joystick(new DirectInput(), j.InstanceGuid))
                .ToArray();

            foreach (var joy in joys)
                joy.Acquire();

            while (!_Accepted)
            {

                foreach (var joy in joys)
                {
                    joy.Poll();
                    var state = joy.GetCurrentState();
                    var pressedButton = Array.IndexOf(state.Buttons, true);
                    if (pressedButton > -1)
                    {
                        JoyName = joy.Information.InstanceName;
                        JoyKey = pressedButton;
                        JoyNameText.Text = JoyName;
                        ButtonNumberText.Text = "Кнопка " + JoyKey;
                        CommandManager.InvalidateRequerySuggested();
                        break;
                    }
                }


                await Task.Delay(100);
            }
        }

        #region Command AcceptButtonCommand - Принять изменения

        /// <summary>Принять изменения</summary>
        private Command _AcceptButtonCommand;

        /// <summary>Принять изменения</summary>
        public Command AcceptButtonCommand => _AcceptButtonCommand
            ??= new Command(OnAcceptButtonCommandExecuted, CanAcceptButtonCommandExecute, "Принять изменения");

        /// <summary>Проверка возможности выполнения - Принять изменения</summary>
        private bool CanAcceptButtonCommandExecute() => JoyName != null && JoyKey > -1;

        /// <summary>Логика выполнения - Принять изменения</summary>
        private void OnAcceptButtonCommandExecuted()
        {
            _Accepted = true;
            DialogResult = true;
        }

        #endregion
    }
}

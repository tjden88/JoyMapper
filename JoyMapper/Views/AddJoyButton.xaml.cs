using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Models;
using SharpDX.DirectInput;
using WPR.MVVM.Commands;

namespace JoyMapper.Views
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


        public string JoyName { get; set; }

        public JoyAction JoyAction { get; set; }

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
                    //Debug.WriteLine(joy.Information.InstanceName);
                    var state = joy.GetCurrentState();


                    // Проверка кнопок
                    var pressedButton = Array.IndexOf(state.Buttons, true) + 1;
                    if (pressedButton > 0)
                    {
                        JoyName = joy.Information.InstanceName;
                        JoyAction = new JoyAction(JoyAction.StateType.Button)
                        {
                            ButtonNumber = pressedButton
                        };
                        JoyNameText.Text = JoyName;
                        ButtonActionText.Text = "Кнопка " + pressedButton;
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
        private bool CanAcceptButtonCommandExecute() => JoyName != null && JoyAction != null;

        /// <summary>Логика выполнения - Принять изменения</summary>
        private void OnAcceptButtonCommandExecuted()
        {
            _Accepted = true;
            DialogResult = true;
        }

        #endregion
    }
}

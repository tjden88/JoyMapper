using System;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Models;
using SharpDX.DirectInput;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class AddJoyActionViewModel : ViewModel
    {

        #region Prop

        #region JoyName : string - Имя джойстика

        /// <summary>Имя джойстика</summary>
        private string _JoyName = "Джойстик...";

        /// <summary>Имя джойстика</summary>
        public string JoyName
        {
            get => _JoyName;
            set => Set(ref _JoyName, value);
        }

        #endregion


        #region JoyAction : JoyAction - Назначаемое действие джойстика

        /// <summary>Назначаемое действие джойстика</summary>
        private JoyAction _JoyAction;

        /// <summary>Назначаемое действие джойстика</summary>
        public JoyAction JoyAction
        {
            get => _JoyAction;
            set => IfSet(ref _JoyAction, value)
                .CallPropertyChanged(nameof(JoyActionName));
        }

        #endregion


        /// <summary>Имя действия джойстика</summary>
        public string JoyActionName => JoyAction?.ActionText() ?? "Действие...";


        #region Accepted : bool - Принять привязку

        /// <summary>Принять привязку</summary>
        private bool _Accepted;

        /// <summary>Принять привязку</summary>
        public bool Accepted
        {
            get => _Accepted;
            private set => Set(ref _Accepted, value);
        }

        #endregion


        #endregion

        #region Commands

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
            Accepted = true;
        }


        #endregion


        #region Command LoadConnectedJoysticksCommand - Загрузить подключённые джойстики для отслеживания

        /// <summary>Загрузить подключённые джойстики для отслеживания</summary>
        private Command _LoadConnectedJoysticksCommand;

        /// <summary>Загрузить подключённые джойстики для отслеживания</summary>
        public Command LoadConnectedJoysticksCommand => _LoadConnectedJoysticksCommand
            ??= new Command(OnLoadConnectedJoysticksCommandExecuted, CanLoadConnectedJoysticksCommandExecute, "Загрузить подключённые джойстики для отслеживания");

        /// <summary>Проверка возможности выполнения - Загрузить подключённые джойстики для отслеживания</summary>
        private bool CanLoadConnectedJoysticksCommandExecute() => true;

        /// <summary>Логика выполнения - Загрузить подключённые джойстики для отслеживания</summary>
        private void OnLoadConnectedJoysticksCommandExecuted() => ListenConnectedJoysticks();

        #endregion


        #endregion


        private async void ListenConnectedJoysticks()
        {
            var connectedDevices = new DirectInput()
                .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

            var joys = connectedDevices
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

                    // Проверка POW
                    if (state.PointOfViewControllers[0] > -1)
                    {
                        JoyName = joy.Information.InstanceName;
                        JoyAction = new JoyAction
                        {
                            Type = JoyAction.StateType.POW1,
                            POWPosition = state.PointOfViewControllers[0]
                        };
                        break;
                    }
                    if (state.PointOfViewControllers[1] > -1)
                    {
                        JoyName = joy.Information.InstanceName;
                        JoyAction = new JoyAction
                        {
                            Type = JoyAction.StateType.POW2,
                            POWPosition = state.PointOfViewControllers[1]
                        };
                        break;
                    }


                    // Проверка кнопок
                    var pressedButton = Array.IndexOf(state.Buttons, true) + 1;
                    if (pressedButton > 0)
                    {
                        JoyName = joy.Information.InstanceName;
                        JoyAction = new JoyAction
                        {
                            Type = JoyAction.StateType.Button,
                            ButtonNumber = pressedButton
                        };
                        break;
                    }
                }


                await Task.Delay(100);
            }
        }

    }
}

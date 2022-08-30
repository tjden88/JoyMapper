using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;
using JoyMapper.Services;
using SharpDX.DirectInput;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class AddJoyActionViewModel : ViewModel, IDisposable
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
        private JoyActionBase _JoyAction;

        /// <summary>Назначаемое действие джойстика</summary>
        public JoyActionBase JoyAction
        {
            get => _JoyAction;
            set => IfSet(ref _JoyAction, value)
                .CallPropertyChanged(nameof(JoyActionName));
        }

        #endregion


        /// <summary>Имя действия джойстика</summary>
        public string JoyActionName => JoyAction?.Description ?? "Действие...";


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

        private bool _Listen = true;
        private async void ListenConnectedJoysticks()
        {

            var connectedDevices = new DirectInput()
                  .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);


            var pollers = connectedDevices
                .Select(j => new JoystickPoller(j.InstanceName, AllJoyActions()))
                .ToArray();


            foreach (var poller in pollers)
            {
              await poller.SyncActions();
            }


            while (_Listen)
            {
                foreach (var poller in pollers)
                {
                    var diff = poller.GetActiveDifferents();
                    if (diff.Any())
                    {
                        JoyName = poller.JoystickName;
                        JoyAction = diff.First();

                        CommandManager.InvalidateRequerySuggested();
                        break;
                    }
                }
                await Task.Delay(100);
            }

            foreach (var poller in pollers) poller.Dispose();
        }


        #region AllActions

        private static List<JoyActionBase> AllJoyActions()
        {
            var list = new List<JoyActionBase>();

            // Кнопки
            for (var i = 1; i <= 128; i++)
            {
                list.Add(new SimpleButtonJoyAction()
                {
                    Button = new JoyButton() { Type = ButtonType.Button, Value = i },
                });
            }

            var powValues = new[] { 0, 4500, 9000, 13500, 18000, 22500, 27000, 31500 };

            // POW
            foreach (var powValue in powValues)
            {
                list.Add(new SimpleButtonJoyAction()
                {
                    Button = new JoyButton() { Type = ButtonType.Pow1, Value = powValue },
                });

                list.Add(new SimpleButtonJoyAction()
                {
                    Button = new JoyButton() { Type = ButtonType.Pow2, Value = powValue },
                });
            }

            // Оси
            foreach (AxisJoyAction.Axises axis in Enum.GetValues(typeof(AxisJoyAction.Axises)))
            {
                list.Add(new AxisJoyAction()
                {
                    Axis = axis,
                    StartValue = 0,
                    EndValue = 20000,
                });
                list.Add(new AxisJoyAction()
                {
                    Axis = axis,
                    StartValue = 45000,
                    EndValue = 65535,
                });
            }

            return list;
        }

        #endregion

        public void Dispose() => _Listen = false;
    }
}

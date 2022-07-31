using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
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
        public string JoyActionName => JoyAction?.ActionText ?? "Действие...";


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

            var joyStates = joys.Select(j => new JoyState()
            {
                Joystick = j,
                Actions = AllActions(),
            }).ToArray();

            foreach (var js in joyStates)
                js.SyncStatus();


            while (!_Accepted)
            {
                foreach (var joyState in joyStates)
                {
                    var diff = joyState.GetDifferents();
                    if (diff.FirstOrDefault(d => d.IsActive) is not { } firstDiff) continue;

                    JoyName = joyState.Joystick.Information.InstanceName;
                    JoyAction = firstDiff.Action;
                    CommandManager.InvalidateRequerySuggested();
                    break;

                }
                await Task.Delay(100);
            }

            foreach (var joy in joys) joy.Unacquire();
        }

        private class JoyAxisState
        {
            public string JoyName { get; set; }

            public int XAxis { get; set; }

            public int YAxis { get; set; }

            public int ZAxis { get; set; }

            public int RxAxis { get; set; }

            public int RyAxis { get; set; }

            public int RzAxis { get; set; }

            public int Slider1 { get; set; }
            public int Slider2 { get; set; }

            public void Update(ref JoystickState joyState)
            {
                XAxis = joyState.X;
                YAxis = joyState.Y;
                ZAxis = joyState.Z;
                RxAxis = joyState.RotationX;
                RyAxis = joyState.RotationY;
                RzAxis = joyState.RotationZ;
                Slider1 = joyState.Sliders[0];
                Slider2 = joyState.Sliders[1];
            }

        }

        private static List<JoyState.ActionState> AllActions()
        {
            var list = new List<JoyState.ActionState>();

            for (var i = 1; i <= 128; i++)
            {
                list.Add(new(new JoyAction
                {
                    Type = JoyAction.StateType.Button,
                    ButtonNumber = i,
                }));
            }


            return list;
        }

    }
}

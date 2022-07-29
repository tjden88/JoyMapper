using System;
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
            var state = new JoystickState();

            var connectedDevices = new DirectInput()
                  .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly);

            var joys = connectedDevices
                .Select(j => new Joystick(new DirectInput(), j.InstanceGuid))
                .ToArray();


            var joyAxisStates = joys
                .Select(j => new JoyAxisState
                {
                    JoyName = j.Information.InstanceName
                })
                .ToArray();

            foreach (var joy in joys)
            {
                joy.Acquire();
                joy.Poll();
            }

            await Task.Delay(50);

            foreach (var joy in joys)
            {
                joy.GetCurrentState(ref state);
                joyAxisStates.First(s => s.JoyName == joy.Information.InstanceName)
                    .Update(ref state);
            }

            var axisCheckCounter = 0;


            while (!_Accepted)
            {
                axisCheckCounter++;

                foreach (var joy in joys)
                {
                    joy.GetCurrentState(ref state);

                    // Проверка смещения осей каждые 500 мс
                    if (axisCheckCounter > 4)
                    {
                        const int axisOffset = 20000;

                        JoyAction MakeAxisAction(JoyAction.Axises axises)
                        {
                            return new JoyAction()
                            {
                                Type = JoyAction.StateType.Axis,
                                Axis = axises
                            };
                        }

                        var prevState = joyAxisStates.First(s => s.JoyName == joy.Information.InstanceName);

                        var axisChanged = false;

                        // Проверить оси
                        if (Math.Abs(prevState.XAxis - state.X) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.X);
                            axisChanged = true;
                        }
                        if (Math.Abs(prevState.YAxis - state.Y) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.Y);
                            axisChanged = true;
                        }
                        if (Math.Abs(prevState.ZAxis - state.Z) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.Z);
                            axisChanged = true;
                        }
                        if (Math.Abs(prevState.RxAxis - state.RotationX) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.RX);
                            axisChanged = true;
                        }
                        if (Math.Abs(prevState.RyAxis - state.RotationY) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.RY);
                            axisChanged = true;
                        }
                        if (Math.Abs(prevState.RzAxis - state.RotationZ) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.RZ);
                            axisChanged = true;
                        }
                        if (Math.Abs(prevState.Slider1 - state.Sliders[0]) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.Slider1);
                            axisChanged = true;
                        }
                        if (Math.Abs(prevState.Slider2 - state.Sliders[1]) > axisOffset)
                        {
                            JoyName = joy.Information.InstanceName;
                            JoyAction = MakeAxisAction(JoyAction.Axises.Slider2);
                            axisChanged = true;
                        }

                        // Задать текущее состояние
                        prevState.Update(ref state);

                        if (axisChanged)
                        {
                            CommandManager.InvalidateRequerySuggested();
                            break;
                        }

                    }

                    // Проверка POW
                    if (state.PointOfViewControllers[0] > -1)
                    {
                        JoyName = joy.Information.InstanceName;
                        JoyAction = new JoyAction
                        {
                            Type = JoyAction.StateType.POW1,
                            POWPosition = state.PointOfViewControllers[0]
                        };
                        CommandManager.InvalidateRequerySuggested();
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
                        CommandManager.InvalidateRequerySuggested();
                        break;
                    }


                    // Проверка кнопок
                    var pressedButton = Array.LastIndexOf(state.Buttons, true) + 1;
                    if (pressedButton <= 0) continue;

                    JoyName = joy.Information.InstanceName;
                    JoyAction = new JoyAction
                    {
                        Type = JoyAction.StateType.Button,
                        ButtonNumber = pressedButton
                    };
                    CommandManager.InvalidateRequerySuggested();
                    break;
                }

                await Task.Delay(100);

                if (axisCheckCounter > 4) axisCheckCounter = 0;
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

    }
}

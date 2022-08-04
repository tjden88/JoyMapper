using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Services;
using JoyMapper.ViewModels.JoyActions;
using SharpDX.DirectInput;
using WPR;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class AddPatternViewModel : WindowViewModel
    {


        public AddPatternViewModel()
        {
            Task.Run(CheckActionStatus);
        }

        #region Props


        #region ChangesSaved : bool - Изменения приняты

        /// <summary>Изменения приняты</summary>
        private bool _ChangesSaved;

        /// <summary>Изменения приняты</summary>
        public bool ChangesSaved
        {
            get => _ChangesSaved;
            private set => Set(ref _ChangesSaved, value);
        }

        #endregion


        #region PatternName : string - Имя паттерна

        /// <summary>Имя паттерна</summary>
        private string _PatternName;

        /// <summary>Имя паттерна</summary>
        public string PatternName
        {
            get => _PatternName;
            set => Set(ref _PatternName, value);
        }

        #endregion


        #region JoyName : string - Имя назначенного джойстика

        /// <summary>Имя назначенного джойстика</summary>
        private string _JoyName;

        /// <summary>Имя назначенного джойстика</summary>
        public string JoyName
        {
            get => _JoyName;
            set => IfSet(ref _JoyName, value).CallPropertyChanged(nameof(JoyButtonText));
        }

        #endregion


        #region JoyActionOld : JoyActionOld - Действие джойстика

        /// <summary>Действие джойстика</summary>
        private JoyActionOld _JoyActionOld;

        /// <summary>Действие джойстика</summary>
        public JoyActionOld JoyActionOld
        {
            get => _JoyActionOld;
            set => IfSet(ref _JoyActionOld, value)
                .CallPropertyChanged(nameof(JoyButtonText))
                .CallPropertyChanged(nameof(AxisSettingsEnable));
        }

        #endregion


        #region ActionIsActive : bool - Назначенное действие джойстика сейчас активно

        /// <summary>Назначенное действие джойстика сейчас активно</summary>
        private bool _ActionIsActive;

        /// <summary>Назначенное действие джойстика сейчас активно</summary>
        public bool ActionIsActive
        {
            get => _ActionIsActive;
            set => IfSet(ref _ActionIsActive, value)
                .CallPropertyChanged(nameof(ActionIsActiveText));
        }

        #endregion


        /// <summary> Текст активности назначенного действия </summary>
        public string ActionIsActiveText => ActionIsActive
            ? "Активно"
            : "Неактивно";


        /// <summary> Назначенное действие </summary>
        public string JoyButtonText => JoyName is null
            ? "-не определено-"
            : JoyName + " - " + JoyActionOld?.ActionText;


        #region CurrentAxisValue : int - Текущее положение оси

        /// <summary>Текущее положение оси</summary>
        private int _CurrentAxisValue;

        /// <summary>Текущее положение оси</summary>
        public int CurrentAxisValue
        {
            get => _CurrentAxisValue;
            private set => Set(ref _CurrentAxisValue, value);
        }

        #endregion


        #endregion



        #region ActionsViewModels


        #region SimpleButtonJoyActionViewModel : SimpleButtonJoyActionViewModel - Вьюмодель простого действия кнопки

        /// <summary>Вьюмодель простого действия кнопки</summary>
        private SimpleButtonJoyActionViewModel _SimpleButtonJoyActionViewModel = new();

        /// <summary>Вьюмодель простого действия кнопки</summary>
        public SimpleButtonJoyActionViewModel SimpleButtonJoyActionViewModel
        {
            get => _SimpleButtonJoyActionViewModel;
            set => Set(ref _SimpleButtonJoyActionViewModel, value);
        }

        #endregion

        #region ExtendedButtonJoyActionViewModel : ExtendedButtonJoyActionViewModel - Вьюмодель расширенного действия кнопки

        /// <summary>Вьюмодель расширенного действия кнопки</summary>
        private ExtendedButtonJoyActionViewModel _ExtendedButtonJoyActionViewModel = new();

        /// <summary>Вьюмодель расширенного действия кнопки</summary>
        public ExtendedButtonJoyActionViewModel ExtendedButtonJoyActionViewModel
        {
            get => _ExtendedButtonJoyActionViewModel;
            set => Set(ref _ExtendedButtonJoyActionViewModel, value);
        }

        #endregion

        #region AxisJoyActionViewModel : AxisJoyActionViewModel - Вьюмодель действия оси

        /// <summary>Вьюмодель действия оси</summary>
        private AxisJoyActionViewModel _AxisJoyActionViewModel = new();

        /// <summary>Вьюмодель действия оси</summary>
        public AxisJoyActionViewModel AxisJoyActionViewModel
        {
            get => _AxisJoyActionViewModel;
            set => Set(ref _AxisJoyActionViewModel, value);
        }

        #endregion


        #region SelectedJoyAction : JoyActionViewModelBase - Текущая вьюмодель действия

        /// <summary>Текущая вьюмодель действия</summary>
        private JoyActionViewModelBase _SelectedJoyAction;

        /// <summary>Текущая вьюмодель действия</summary>
        public JoyActionViewModelBase SelectedJoyAction
        {
            get => _SelectedJoyAction;
            set => Set(ref _SelectedJoyAction, value);
        }

        #endregion

        

        #endregion


        #region Commands

        #region Command AttachButtonIfEmptyCommand - Назначить кнопку джойстика если не назначена

        /// <summary>Назначить кнопку джойстика если не назначена</summary>
        private Command _AttachButtonIfEmptyCommand;

        /// <summary>Назначить кнопку джойстика если не назначена</summary>
        public Command AttachButtonIfEmptyCommand => _AttachButtonIfEmptyCommand
            ??= new Command(OnAttachJoyButtonCommandExecuted, CanAttachButtonIfEmptyCommandExecute, "Назначить кнопку джойстика если не назначена");

        /// <summary>Проверка возможности выполнения - Назначить кнопку джойстика если не назначена</summary>
        private bool CanAttachButtonIfEmptyCommandExecute() => JoyActionOld == null;


        #endregion


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
            var joyAction = new JoyActionAdderService().MapJoyAction(out var joyName);
            if (joyAction == null) return;

            if (joyAction.Type == JoyActionOld.StateType.Axis)
            {
                joyAction.StartAxisValue = AxisStartValue;
                joyAction.EndAxisValue = AxisFinishValue;
            }
            JoyActionOld = joyAction;
            JoyName = joyName;
        }

        #endregion



        #region AsyncCommand SaveCommand - Сохранить паттерн

        /// <summary>Сохранить паттерн</summary>
        private AsyncCommand _SaveCommand;

        /// <summary>Сохранить паттерн</summary>
        public AsyncCommand SaveCommand => _SaveCommand
            ??= new AsyncCommand(OnSaveCommandExecutedAsync, CanSaveCommandExecute, "Сохранить паттерн");

        /// <summary>Проверка возможности выполнения - Сохранить паттерн</summary>
        private bool CanSaveCommandExecute() => true;

        /// <summary>Логика выполнения - Сохранить паттерн</summary>
        private async Task OnSaveCommandExecutedAsync(CancellationToken cancel)
        {
            if (JoyName == null || JoyActionOld == null)
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Не определена кнопка или ось контроллера для назначения паттерна");
                return;
            }

            if (PressKeyBindings.Count == 0 && ReleaseKeyBindings.Count == 0)
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Клавиатурные команды не назначены");
                return;
            }

            if (string.IsNullOrEmpty(PatternName))
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Введите имя паттерна");
                return;
            }

            PatternName = PatternName.Trim();
            ChangesSaved = true;
        }

        #endregion

        #endregion

        // Проверяет сатус назначеного действия
        private async Task CheckActionStatus()
        {
            var joystickState = new JoystickState();
            while (!ChangesSaved)
            {
                if (JoyActionOld != null)
                {
                    var instance = new DirectInput()
                        .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                        .FirstOrDefault(d => d.ProductName == JoyName);
                    if (instance != null)
                    {
                        var joy = new Joystick(new DirectInput(), instance.InstanceGuid);
                        joy.Acquire();
                        joy.Poll();
                        await Task.Delay(100);
                        joy.GetCurrentState(ref joystickState);
                        var status = JoyActionOld.IsActionActive(ref joystickState);
                        ActionIsActive = status;
                        if (JoyActionOld.Type == JoyActionOld.StateType.Axis)
                        {
                            // Обновить значение оси

                            var newValue = JoyActionOld.Axis switch
                            {
                                JoyActionOld.Axises.X => joystickState.X,
                                JoyActionOld.Axises.Y => joystickState.Y,
                                JoyActionOld.Axises.Z => joystickState.Z,
                                JoyActionOld.Axises.RX => joystickState.RotationX,
                                JoyActionOld.Axises.RY => joystickState.RotationY,
                                JoyActionOld.Axises.RZ => joystickState.RotationZ,
                                JoyActionOld.Axises.Slider1 => joystickState.Sliders[0],
                                JoyActionOld.Axises.Slider2 => joystickState.Sliders[1],
                                _ => throw new ArgumentOutOfRangeException()
                            };
                            CurrentAxisValue = newValue;
                        }

                        joy.Unacquire();
                    }
                }
            }
        }

    }
}

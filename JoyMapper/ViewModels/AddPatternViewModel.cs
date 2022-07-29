using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models;
using JoyMapper.Services;
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

        
        #region IsPressRecorded : bool - Нажата ли кнопка записи нажатий

        /// <summary>Нажата ли кнопка записи нажатий</summary>
        private bool _IsPressRecorded;

        /// <summary>Нажата ли кнопка записи нажатий</summary>
        public bool IsPressRecorded
        {
            get => _IsPressRecorded;
            set => IfSet(ref _IsPressRecorded, value).CallPropertyChanged(nameof(PressButtonText));
        }

        #endregion


        #region IsReleaseRecorded : bool - Нажата ли кнопка записи при отпускании

        /// <summary>Нажата ли кнопка записи при отпускании</summary>
        private bool _IsReleaseRecorded;

        /// <summary>Нажата ли кнопка записи при отпускании</summary>
        public bool IsReleaseRecorded
        {
            get => _IsReleaseRecorded;
            set => IfSet(ref _IsReleaseRecorded, value).CallPropertyChanged(nameof(ReleaseButtonText));
        }

        #endregion


        #region PressButtonText : string - Текст кнопки записи нажатия

        /// <summary>Текст кнопки записи нажатия</summary>
        public string PressButtonText => IsPressRecorded ? "Остановть запись" : "Начать запись";

        #endregion


        #region ReleaseButtonText : string - Текст кнопки записи отпускания

        /// <summary>Текст кнопки записи отпускания</summary>
        public string ReleaseButtonText => IsReleaseRecorded ? "Остановть запись" : "Начать запись";

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


        #region JoyAction : JoyAction - Действие джойстика

        /// <summary>Действие джойстика</summary>
        private JoyAction _JoyAction;

        /// <summary>Действие джойстика</summary>
        public JoyAction JoyAction
        {
            get => _JoyAction;
            set => IfSet(ref _JoyAction, value).CallPropertyChanged(nameof(JoyButtonText));
        }

        #endregion


        #region ActionIsActive : bool - Назначенное действие джойстика сейчас активно

        /// <summary>Назначенное действие джойстика сейчас активно</summary>
        private bool _ActionIsActive;

        /// <summary>Назначенное действие джойстика сейчас активно</summary>
        public bool ActionIsActive
        {
            get => _ActionIsActive;
            set => Set(ref _ActionIsActive, value);
        }

        #endregion

        
        

        /// <summary> Назначенное действие </summary>
        public string JoyButtonText => JoyName is null
            ? "-не определено-"
            : JoyName + " - " + JoyAction.ActionText(); 


        #region PressKeyBindings : ObservableCollection<KeyboardKeyBinding> - Список команд при нажатии кнопки

        /// <summary>Список команд при нажатии кнопки</summary>
        private ObservableCollection<KeyboardKeyBinding> _PressKeyBindings = new();

        /// <summary>Список команд при нажатии кнопки</summary>
        public ObservableCollection<KeyboardKeyBinding> PressKeyBindings
        {
            get => _PressKeyBindings;
            set => Set(ref _PressKeyBindings, value);
        }

        #endregion


        #region ReleaseKeyBindings : ObservableCollection<KeyboardKeyBinding> - Список команд при отпускании кнопки

        /// <summary>Список команд при отпускании кнопки</summary>
        private ObservableCollection<KeyboardKeyBinding> _ReleaseKeyBindings = new();

        /// <summary>Список команд при отпускании кнопки</summary>
        public ObservableCollection<KeyboardKeyBinding> ReleaseKeyBindings
        {
            get => _ReleaseKeyBindings;
            set => Set(ref _ReleaseKeyBindings, value);
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
        private bool CanAttachButtonIfEmptyCommandExecute() => JoyAction == null;


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
            
            JoyAction = joyAction;
            JoyName = joyName;
        }

        #endregion


        #region Command ChangePressRecordCommand - Начать / остановить запись клавиатурных команд при нажатии кнопки джойстика

        /// <summary>Начать / остановить запись клавиатурных команд при нажатии кнопки джойстика</summary>
        private Command _ChangePressRecordCommand;

        /// <summary>Начать / остановить запись клавиатурных команд при нажатии кнопки джойстика</summary>
        public Command ChangePressRecordCommand => _ChangePressRecordCommand
            ??= new Command(OnChangePressRecordCommandExecuted, CanChangePressRecordCommandExecute, "Начать / остановить запись клавиатурных команд при нажатии кнопки джойстика");

        /// <summary>Проверка возможности выполнения - Начать / остановить запись клавиатурных команд при нажатии кнопки джойстика</summary>
        private bool CanChangePressRecordCommandExecute() => true;

        /// <summary>Логика выполнения - Начать / остановить запись клавиатурных команд при нажатии кнопки джойстика</summary>
        private void OnChangePressRecordCommandExecuted() => IsPressRecorded = !IsPressRecorded;

        #endregion


        #region Command ChangeReleaseRecordCommand - Начать / остановить запись клавиатурных команд при отпускании кнопки джойстика

        /// <summary>Начать / остановить запись клавиатурных команд при отпускании кнопки джойстика</summary>
        private Command _ChangeReleaseRecordCommand;

        /// <summary>Начать / остановить запись клавиатурных команд при отпускании кнопки джойстика</summary>
        public Command ChangeReleaseRecordCommand => _ChangeReleaseRecordCommand
            ??= new Command(OnChangeReleaseRecordCommandExecuted, CanChangeReleaseRecordCommandExecute, "Начать / остановить запись клавиатурных команд при отпускании кнопки джойстика");

        /// <summary>Проверка возможности выполнения - Начать / остановить запись клавиатурных команд при отпускании кнопки джойстика</summary>
        private bool CanChangeReleaseRecordCommandExecute() => true;

        /// <summary>Логика выполнения - Начать / остановить запись клавиатурных команд при отпускании кнопки джойстика</summary>
        private void OnChangeReleaseRecordCommandExecuted() => IsReleaseRecorded = !IsReleaseRecorded;

        #endregion


        #region Command ClaerPressBindingsCommand - Очистить назначения команд нажатия

        /// <summary>Очистить назначения команд нажатия</summary>
        private Command _ClaerPressBindingsCommand;

        /// <summary>Очистить назначения команд нажатия</summary>
        public Command ClaerPressBindingsCommand => _ClaerPressBindingsCommand
            ??= new Command(OnClaerPressBindingsCommandExecuted, CanClaerPressBindingsCommandExecute, "Очистить назначения команд нажатия");

        /// <summary>Проверка возможности выполнения - Очистить назначения команд нажатия</summary>
        private bool CanClaerPressBindingsCommandExecute() => PressKeyBindings?.Count > 0;

        /// <summary>Логика выполнения - Очистить назначения команд нажатия</summary>
        private void OnClaerPressBindingsCommandExecuted() => PressKeyBindings.Clear();

        #endregion


        #region Command ClaerReleaseBindingsCommand - Очистить назначения команд отпускания

        /// <summary>Очистить назначения команд отпускания</summary>
        private Command _ClaerReleaseBindingsCommand;

        /// <summary>Очистить назначения команд отпускания</summary>
        public Command ClaerReleaseBindingsCommand => _ClaerReleaseBindingsCommand
            ??= new Command(OnClaerReleaseBindingsCommandExecuted, CanClaerReleaseBindingsCommandExecute, "Очистить назначения команд отпускания");

        /// <summary>Проверка возможности выполнения - Очистить назначения команд отпускания</summary>
        private bool CanClaerReleaseBindingsCommandExecute() => ReleaseKeyBindings?.Count > 0;

        /// <summary>Логика выполнения - Очистить назначения команд отпускания</summary>
        private void OnClaerReleaseBindingsCommandExecuted() => ReleaseKeyBindings.Clear();

        #endregion


        #region Command RemoveKeyBindingCommand : KeyboardKeyBinding - Удалить назначение клавиши

        /// <summary>Удалить назначение клавиши</summary>
        private Command<KeyboardKeyBinding> _RemoveKeyBindingCommand;

        /// <summary>Удалить назначение клавиши</summary>
        public Command<KeyboardKeyBinding> RemoveKeyBindingCommand => _RemoveKeyBindingCommand
            ??= new Command<KeyboardKeyBinding>(OnRemoveKeyBindingCommandExecuted, CanRemoveKeyBindingCommandExecute, "Удалить назначение клавиши");

        /// <summary>Проверка возможности выполнения - Удалить назначение клавиши</summary>
        private bool CanRemoveKeyBindingCommandExecute(KeyboardKeyBinding p) => true;

        /// <summary>Проверка возможности выполнения - Удалить назначение клавиши</summary>
        private void OnRemoveKeyBindingCommandExecuted(KeyboardKeyBinding p)
        {
            PressKeyBindings.Remove(p);
            ReleaseKeyBindings.Remove(p);
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
            if (JoyName == null || JoyAction == null)
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
            while (true)
            {
                if (JoyAction != null)
                {
                    var instance = new DirectInput()
                        .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                        .FirstOrDefault(d=>d.ProductName == JoyName);
                    if (instance != null)
                    {
                        var joy = new Joystick(new DirectInput(), instance.InstanceGuid);
                        joy.Acquire();
                        await Task.Delay(10);
                        joy.GetCurrentState(ref joystickState);
                        var status = JoyAction.IsActionActive(ref joystickState);
                        ActionIsActive = status;

                        joy.Unacquire();
                    }
                }
                await Task.Delay(100);
            }
        }

    }
}

using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Helpers;
using JoyMapper.Services;
using JoyMapper.Services.ActionWatchers;
using JoyMapper.ViewModels.JoyActions;
using SharpDX.DirectInput;
using WPR;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class AddPatternViewModel : WindowViewModel
    {

        private ActionWatcherBase _CurrentActionWatcher;

        public AddPatternViewModel() => Task.Run(CheckActionStatus);

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


        #region JoyName : string - Имя джойстика

        /// <summary>Имя джойстика</summary>
        private string _JoyName;

        /// <summary>Имя джойстика</summary>
        public string JoyName
        {
            get => _JoyName;
            set => Set(ref _JoyName, value);
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


        #region JoyAction : JoyActionViewModelBase - Текущая вьюмодель действия

        /// <summary>Текущая вьюмодель действия</summary>
        private JoyActionViewModelBase _JoyAction;

        /// <summary>Текущая вьюмодель действия</summary>
        public JoyActionViewModelBase JoyAction
        {
            get => _JoyAction;
            set => IfSet(ref _JoyAction, value)
                .Then(v => 
                    _CurrentActionWatcher = v is null 
                        ? null 
                        : ActionWatcherFactory.CreateActionWatcherBase(v.ToModel()));
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

            // TODO: не работает
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
            if (JoyAction == null)
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Не определена кнопка или ось контроллера для назначения паттерна");
                return;
            }

            if (!JoyAction.HasKeyBindings)
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Клавиатурные команды не назначены");
                return;
            }

            var patternName = PatternName?.Trim();

            if (string.IsNullOrEmpty(patternName))
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Введите имя паттерна");
                return;
            }

            PatternName = patternName;
            ChangesSaved = true;
        }

        #endregion

        #endregion

        // Проверяет сатус назначеного действия
        private async Task CheckActionStatus()
        {
            while (!ChangesSaved)
            {
                if (_CurrentActionWatcher == null)
                {
                    await Task.Delay(200);
                    continue;
                }


                var instance = new DirectInput()
                    .GetDevices(DeviceClass.GameControl, DeviceEnumerationFlags.AttachedOnly)
                    .FirstOrDefault(d => d.ProductName == JoyName);

                if (instance == null) continue;

                var joy = new Joystick(new DirectInput(), instance.InstanceGuid);
                joy.Acquire();
                await Task.Delay(100);

                _CurrentActionWatcher.Poll(joy.GetCurrentState().ToModel(), false);
                ActionIsActive = _CurrentActionWatcher.IsActive;


                joy.Unacquire();
            }
        }

    }
}

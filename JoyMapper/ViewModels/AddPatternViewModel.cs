﻿using System.Linq;
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


        #region CurrentJoyAction : JoyActionViewModelBase - Текущая вьюмодель действия

        /// <summary>Текущая вьюмодель действия</summary>
        private JoyActionViewModelBase _CurrentJoyAction;

        /// <summary>Текущая вьюмодель действия</summary>
        public JoyActionViewModelBase CurrentJoyAction
        {
            get => _CurrentJoyAction;
            set => IfSet(ref _CurrentJoyAction, value)
                .Then(v =>
                {
                    if (v is null)
                        _CurrentActionWatcher = null;
                    else
                    {
                        var currentActionWatcher = ActionWatcherFactory.CreateActionWatcherBase(v.ToModel());
                        currentActionWatcher.AlloySendKeyboardCommands = false;
                        _CurrentActionWatcher = currentActionWatcher;
                    }
                });
        }

        #endregion

        private ActionWatcherBase _CurrentActionWatcher;

        #endregion


        #region Commands


        #region Command AttachButtonIfEmptyCommand - Назначить кнопку джойстика если не назначена

        /// <summary>Назначить кнопку джойстика если не назначена</summary>
        private Command _AttachButtonIfEmptyCommand;

        /// <summary>Назначить кнопку джойстика если не назначена</summary>
        public Command AttachButtonIfEmptyCommand => _AttachButtonIfEmptyCommand
            ??= new Command(OnAttachJoyButtonCommandExecuted, CanAttachButtonIfEmptyCommandExecute, "Назначить кнопку джойстика если не назначена");

        /// <summary>Проверка возможности выполнения - Назначить кнопку джойстика если не назначена</summary>
        private bool CanAttachButtonIfEmptyCommandExecute() => CurrentJoyAction == null;


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
            if (CurrentJoyAction == null)
            {
                await WPRMessageBox.InformationAsync(App.ActiveWindow, "Не определена кнопка или ось контроллера для назначения паттерна");
                return;
            }

            if (!CurrentJoyAction.HasKeyBindings)
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
                    .FirstOrDefault(d => d.ProductName == CurrentJoyAction.JoyName);

                if (instance == null) continue;

                var joy = new Joystick(new DirectInput(), instance.InstanceGuid);
                joy.Acquire();
                await Task.Delay(100);

                _CurrentActionWatcher.Poll(joy.GetCurrentState().ToModel());
                ActionIsActive = _CurrentActionWatcher.IsActive;


                joy.Unacquire();
            }
        }

    }
}

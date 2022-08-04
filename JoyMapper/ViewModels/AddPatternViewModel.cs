﻿using System;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Helpers;
using JoyMapper.Models.JoyActions;
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
        public enum ActionType // Тип настраиваемого действия
        {
            None,
            SimpleButton,
            ExtendedButton,
            Axis
        }


        public AddPatternViewModel() => Task.Run(CheckActionStatus);

        /// <summary> Для редактирования паттерна </summary>
        public AddPatternViewModel(JoyActionViewModelBase viewModel) : this()
        {
            switch (viewModel)
            {
                case AxisJoyActionViewModel axis:
                    _AxisJoyActionViewModel = axis;
                    CurrentActionType = ActionType.Axis;
                    break;
                case SimpleButtonJoyActionViewModel sm:
                    _SimpleButtonJoyActionViewModel = sm;
                    _ExtendedButtonJoyActionViewModel.Button = sm.Button;
                    CurrentActionType = ActionType.SimpleButton;
                    break;
                case ExtendedButtonJoyActionViewModel extended:
                    _ExtendedButtonJoyActionViewModel = extended;
                    _SimpleButtonJoyActionViewModel.Button = extended.Button;
                    CurrentActionType = ActionType.ExtendedButton;
                    break;
                default:
                    throw new NotSupportedException("Неизвестный тип");
            }
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

        /// <summary> Текст активности назначенного действия </summary>
        public string ActionIsActiveText => ActionIsActive
            ? "Активно"
            : "Неактивно";


        #endregion


        #region IsExtendedButtonMode : bool - Выбран режим расширенной привязки кнопки

        /// <summary>Выбран режим расширенной привязки кнопки</summary>
        private bool _IsExtendedButtonMode;

        /// <summary>Выбран режим расширенной привязки кнопки</summary>
        public bool IsExtendedButtonMode
        {
            get => _IsExtendedButtonMode;
            set => IfSet(ref _IsExtendedButtonMode, value)
                .Then(v => CurrentActionType = v ? ActionType.ExtendedButton : ActionType.SimpleButton)
                .CallPropertyChanged(nameof(ButtonModeInfo));
        }

        #endregion


        #region ButtonModeInfo

        public string ButtonModeInfo => IsExtendedButtonMode
            ? "Позволяет назначить до трёх разных команд на одну кнопку:\n" +
              "◉ Одиночное нажатие\n" +
              "◉ Двойное нажатие\n" +
              "◉ Долгое нажатие"
            : "Позволяет назначить разные команды на нажатие и отпускание.\n" +
              "◉ Подходит для тумблеров\n";

        #endregion


        #region JoyActionText - string

        public string JoyActionText => $"{JoyName} - {JoyAction?.Description}";

        #endregion


        #region CurrentActionType : ActionType - Тип действия

        /// <summary>Тип действия</summary>
        private ActionType _CurrentActionType = ActionType.None;

        /// <summary>Тип действия</summary>
        public ActionType CurrentActionType
        {
            get => _CurrentActionType;
            private set
            {
                _CurrentActionType = value;
                OnPropertyChanged(nameof(CurrentActionType));

                _CurrentActionWatcher = value != ActionType.None
                    ? ActionWatcherFactory.CreateActionWatcherBase(JoyAction.ToModel())
                    : null;

                OnPropertyChanged(nameof(JoyAction));
                OnPropertyChanged(nameof(IsButton));
            }
        }

        #endregion


        #region IsButton

        public bool IsButton => CurrentActionType is ActionType.SimpleButton or ActionType.ExtendedButton;

        #endregion


        #region JoyAction : JoyActionViewModelBase - Текущая вьюмодель действия


        private readonly SimpleButtonJoyActionViewModel _SimpleButtonJoyActionViewModel = new();

        private readonly ExtendedButtonJoyActionViewModel _ExtendedButtonJoyActionViewModel = new();

        private readonly AxisJoyActionViewModel _AxisJoyActionViewModel = new();


        /// <summary>Текущая вьюмодель действия</summary>
        public JoyActionViewModelBase JoyAction =>
            CurrentActionType switch
            {
                ActionType.Axis => _AxisJoyActionViewModel,
                ActionType.SimpleButton => _SimpleButtonJoyActionViewModel,
                ActionType.ExtendedButton => _ExtendedButtonJoyActionViewModel,
                _ => null
            };


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

            var action = joyAction.ToViewModel();
            if (action is AxisJoyActionViewModel axisJoyActionViewModel)
            {
                _AxisJoyActionViewModel.Axis = axisJoyActionViewModel.Axis;
                CurrentActionType = ActionType.Axis;
            }
            else
            {
                var btnAction = (SimpleButtonJoyActionViewModel)action;
                _SimpleButtonJoyActionViewModel.Button = btnAction.Button;
                _ExtendedButtonJoyActionViewModel.Button = btnAction.Button;
                CurrentActionType = IsExtendedButtonMode ? ActionType.ExtendedButton : ActionType.SimpleButton;
            }

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



        #region WatchStatus

        private ActionWatcherBase _CurrentActionWatcher;


        // Проверяет сатус назначеного действия
        private async Task CheckActionStatus()
        {
            while (!ChangesSaved)
            {
                try
                {
                    if (_CurrentActionWatcher == null)
                    {
                        await Task.Delay(200);
                        continue;
                    }

                    if (_CurrentActionWatcher is AxisActionWatcher axisWatcher)
                    {
                        var action = (AxisJoyAction)axisWatcher.JoyAction;
                        action.EndValue = _AxisJoyActionViewModel.EndValue;
                        action.StartValue = _AxisJoyActionViewModel.StartValue;
                        _AxisJoyActionViewModel.CurrentAxisValue = axisWatcher.CurrentValue;
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
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }
        }

        #endregion
    }
}

using System.Collections.ObjectModel;
using System.Linq;
using JoyMapper.Models;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    /// <summary>
    /// Список назначаемых клавиш на то или иное действие
    /// </summary>
    internal class ActionKeysBindingViewModel : ViewModel
    {

        public ActionKeysBindingViewModel(string name) => Name = name;

        #region KeyBindings : ObservableCollection<KeyboardKeyBinding> - Последовательность нажатия или отпускания клавиш

        /// <summary>Последовательность нажатия или отпускания клавиш</summary>
        private ObservableCollection<KeyboardKeyBinding> _KeyBindings = new();

        /// <summary>Последовательность нажатия или отпускания клавиш</summary>
        public ObservableCollection<KeyboardKeyBinding> KeyBindings
        {
            get => _KeyBindings;
            set => Set(ref _KeyBindings, value);
        }

        #endregion


        #region IsRecorded : bool - Активна ли запись с клавиатуры

        /// <summary>Активна ли запись с клавиатуры</summary>
        private bool _IsRecorded;

        /// <summary>Активна ли запись с клавиатуры</summary>
        public bool IsRecorded
        {
            get => _IsRecorded;
            set => IfSet(ref _IsRecorded, value).CallPropertyChanged(nameof(RecordButtonText));
        }

        #endregion


        #region RecordButtonText : string - Текст кнопки записи нажатия

        /// <summary>Текст кнопки записи нажатия</summary>
        public string RecordButtonText => IsRecorded ? "Остановть запись" : "Начать запись";

        #endregion


        #region Name : string - Имя списка команд

        /// <summary>Имя списка команд</summary>
        public string Name { get; }

        #endregion


        #region Command StartStopRecordCommand - Начать / остановить запись команд с клавиатуры

        /// <summary>Начать / остановить запись команд с клавиатуры</summary>
        private Command _StartStopRecordCommand;

        /// <summary>Начать / остановить запись команд с клавиатуры</summary>
        public Command StartStopRecordCommand => _StartStopRecordCommand
            ??= new Command(OnStartStopRecordCommandExecuted, CanStartStopRecordCommandExecute, "Начать / остановить запись команд с клавиатуры");

        /// <summary>Проверка возможности выполнения - Начать / остановить запись команд с клавиатуры</summary>
        private bool CanStartStopRecordCommandExecute() => true;

        /// <summary>Логика выполнения - Начать / остановить запись команд с клавиатуры</summary>
        private void OnStartStopRecordCommandExecuted() => IsRecorded = !IsRecorded;

        #endregion


        #region Command ClearBindingsCommand - Очистить назначенные клавиши

        /// <summary>Очистить назначенные клавиши</summary>
        private Command _ClearBindingsCommand;

        /// <summary>Очистить назначенные клавиши</summary>
        public Command ClearBindingsCommand => _ClearBindingsCommand
            ??= new Command(OnClearBindingsCommandExecuted, CanClearBindingsCommandExecute, "Очистить назначенные клавиши");

        /// <summary>Проверка возможности выполнения - Очистить назначенные клавиши</summary>
        private bool CanClearBindingsCommandExecute() => KeyBindings.Any();

        /// <summary>Логика выполнения - Очистить назначенные клавиши</summary>
        private void OnClearBindingsCommandExecuted()
        {
            KeyBindings.Clear();
        }

        #endregion


        #region Command RemoveKeyBindingCommand - Удалить действие клавиши

        /// <summary>Удалить действие клавиши</summary>
        private Command _RemoveKeyBindingCommand;

        /// <summary>Удалить действие клавиши</summary>
        public Command RemoveKeyBindingCommand => _RemoveKeyBindingCommand
            ??= new Command(OnRemoveKeyBindingCommandExecuted, CanRemoveKeyBindingCommandExecute, "Удалить действие клавиши");

        /// <summary>Проверка возможности выполнения - Удалить действие клавиши</summary>
        private bool CanRemoveKeyBindingCommandExecute(object p) => p is KeyboardKeyBinding;

        /// <summary>Логика выполнения - Удалить действие клавиши</summary>
        private void OnRemoveKeyBindingCommandExecuted(object p) => KeyBindings.Remove((KeyboardKeyBinding)p);

        #endregion

    }
}

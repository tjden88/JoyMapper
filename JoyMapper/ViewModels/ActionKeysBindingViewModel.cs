using System.Collections.ObjectModel;
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
            set => Set(ref _IsRecorded, value);
        }

        #endregion


        #region Name : string - Имя списка команд

        /// <summary>Имя списка команд</summary>
        private string _Name;

        /// <summary>Имя списка команд</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion


        #region Command ClearBindingsCommand - Очистить назначенные клавиши

        /// <summary>Очистить назначенные клавиши</summary>
        private Command _ClearBindingsCommand;

        /// <summary>Очистить назначенные клавиши</summary>
        public Command ClearBindingsCommand => _ClearBindingsCommand
            ??= new Command(OnClearBindingsCommandExecuted, CanClearBindingsCommandExecute, "Очистить назначенные клавиши");

        /// <summary>Проверка возможности выполнения - Очистить назначенные клавиши</summary>
        private bool CanClearBindingsCommandExecute() => true;

        /// <summary>Логика выполнения - Очистить назначенные клавиши</summary>
        private void OnClearBindingsCommandExecuted()
        {
            KeyBindings.Clear();
        }

        #endregion

    }
}

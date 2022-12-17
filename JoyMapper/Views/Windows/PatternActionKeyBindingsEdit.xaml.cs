using JoyMapper.Models;
using JoyMapper.ViewModels.PatternActions.Base;
using System.Linq;
using System.Windows;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для PatternActionKeyBindingsEdit.xaml
    /// </summary>
    public partial class PatternActionKeyBindingsEdit : Window
    {
        public PatternActionKeyBindingsEditViewModel ViewModel { get; }

        public PatternActionKeyBindingsEdit(PatternActionKeysBindingViewModel ViewModel)
        {
            this.ViewModel = new() {KeysBindingViewModel = ViewModel};
            InitializeComponent();
        }


        public class PatternActionKeyBindingsEditViewModel : ViewModel
        {

            public PatternActionKeysBindingViewModel KeysBindingViewModel { get; init; }


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
            public string RecordButtonText => IsRecorded ? "Остановить запись" : "Начать запись";

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
            private bool CanClearBindingsCommandExecute() => KeysBindingViewModel.KeyBindings.Any();

            /// <summary>Логика выполнения - Очистить назначенные клавиши</summary>
            private void OnClearBindingsCommandExecuted()
            {
                KeysBindingViewModel.KeyBindings.Clear();
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
            private void OnRemoveKeyBindingCommandExecuted(object p) => KeysBindingViewModel.KeyBindings.Remove((KeyboardKeyBinding)p);

            #endregion
        }
    }
}

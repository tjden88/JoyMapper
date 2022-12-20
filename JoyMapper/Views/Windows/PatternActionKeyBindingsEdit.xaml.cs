using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using JoyMapper.Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Views.Windows;

/// <summary>
/// Логика взаимодействия для PatternActionKeyBindingsEdit.xaml
/// </summary>
public partial class PatternActionKeyBindingsEdit : Window
{
    public PatternActionKeyBindingsEditViewModel ViewModel { get; }

    public PatternActionKeyBindingsEdit(IEnumerable<KeyboardKeyBinding> KeyBindings, string name)
    {
        ViewModel = new() {KeyBindings = new (KeyBindings), Name = name};
        InitializeComponent();
    }

    private void ButtonOk_OnClick(object Sender, RoutedEventArgs E)
    {
        DialogResult = true;
    }

    private void AddPattern_OnPreviewKeyDown(object sender, KeyEventArgs e)
    {

        if (!ViewModel.IsRecorded) return;
        if (!e.IsRepeat)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            ViewModel.KeyBindings.Add(new KeyboardKeyBinding
            {
                Action = KeyboardKeyBinding.KeyboardAction.KeyPress,
                KeyCode = key
            });
        }
        e.Handled = true;
    }


    private void AddPattern_OnPreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (!ViewModel.IsRecorded) return;
        if (!e.IsRepeat)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            ViewModel.KeyBindings.Add(new KeyboardKeyBinding
            {
                Action = KeyboardKeyBinding.KeyboardAction.KeyUp,
                KeyCode = key
            });
        }
        e.Handled = true;
    }


    public class PatternActionKeyBindingsEditViewModel : ViewModel
    {
        #region KeyBindings : ObservableCollection<KeyboardKeyBinding> - Команды клавиатуры

        /// <summary>Команды клавиатуры</summary>
        private ObservableCollection<KeyboardKeyBinding> _KeyBindings;

        /// <summary>Команды клавиатуры</summary>
        public ObservableCollection<KeyboardKeyBinding> KeyBindings
        {
            get => _KeyBindings;
            set => Set(ref _KeyBindings, value);
        }

        #endregion


        #region Name : string - Имя команд

        /// <summary>Имя команд</summary>
        private string _Name;

        /// <summary>Имя команд</summary>
        public string Name
        {
            get => _Name;
            set => Set(ref _Name, value);
        }

        #endregion

            
        #region IsRecorded : bool - Активна ли запись с клавиатуры

        /// <summary>Активна ли запись с клавиатуры</summary>
        private bool _IsRecorded = true;

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

    private void UIElement_OnPreviewMouseDown(object Sender, MouseButtonEventArgs E)
    {
        Debug.WriteLine(E.ChangedButton);
    }
    private void UIElement_OnPreviewMouseUp(object Sender, MouseButtonEventArgs E)
    {
        Debug.WriteLine(E.ChangedButton);
    }

    private void UIElement_OnMouseWheel(object Sender, MouseWheelEventArgs E)
    {
        Debug.WriteLine(E.Delta);
    }
}
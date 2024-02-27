using System.Collections.Generic;
using System.Collections.ObjectModel;
using JoyMapper.Models;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Interfaces;
using WPR.MVVM.Commands.Base;
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
        ViewModel = new()
        {
            KeyBindings = new (KeyBindings.Select(b => new KeyBindingViewModel(b))),
            Name = name
        };
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
            var kb = new KeyboardKeyBinding
            {
                Action = KeyboardKeyBinding.KeyboardAction.KeyPress,
                KeyCode = key
            };
            ViewModel.KeyBindings.Add(new KeyBindingViewModel(kb));
        }
        e.Handled = true;
    }


    private void AddPattern_OnPreviewKeyUp(object sender, KeyEventArgs e)
    {
        if (!ViewModel.IsRecorded) return;
        if (!e.IsRepeat)
        {
            var key = e.Key == Key.System ? e.SystemKey : e.Key;
            var kb = new KeyboardKeyBinding
            {
                Action = KeyboardKeyBinding.KeyboardAction.KeyUp,
                KeyCode = key
            };
            ViewModel.KeyBindings.Add(new KeyBindingViewModel(kb));
        }
        e.Handled = true;
    }

    private void UIElement_OnPreviewMouseDown(object Sender, MouseButtonEventArgs E)
    {
        if (!ViewModel.IsRecorded) return;
        var kb = new KeyboardKeyBinding
        {
            Action = KeyboardKeyBinding.KeyboardAction.MousePress,
            MouseButton = E.ChangedButton
        };
        ViewModel.KeyBindings.Add(new KeyBindingViewModel(kb));
    }

    private void UIElement_OnPreviewMouseUp(object Sender, MouseButtonEventArgs E)
    {
        if (!ViewModel.IsRecorded) return;
        var kb = new KeyboardKeyBinding
        {
            Action = KeyboardKeyBinding.KeyboardAction.MouseUp,
            MouseButton = E.ChangedButton
        };
        ViewModel.KeyBindings.Add(new KeyBindingViewModel(kb));
    }

    private void UIElement_OnMouseWheel(object Sender, MouseWheelEventArgs E)
    {
        if (!ViewModel.IsRecorded) return;

        var up = E.Delta > 0;

        var kb = new KeyboardKeyBinding
        {
            Action = up ? KeyboardKeyBinding.KeyboardAction.MouseScrollUp : KeyboardKeyBinding.KeyboardAction.MouseScrollDown,
        };
        ViewModel.KeyBindings.Add(new KeyBindingViewModel(kb));
    }

    public class PatternActionKeyBindingsEditViewModel : ViewModel
    {
        #region KeyBindings : ObservableCollection<KeyBindingViewModel> - Команды клавиатуры

        /// <summary>Команды клавиатуры</summary>
        private ObservableCollection<KeyBindingViewModel> _KeyBindings;

        /// <summary>Команды клавиатуры</summary>
        public ObservableCollection<KeyBindingViewModel> KeyBindings
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
            ??= new Command(OnRemoveKeyBindingCommandExecuted, CanRemoveKeyBindingCommandExecute, "Удалить действие");

        /// <summary>Проверка возможности выполнения - Удалить действие клавиши</summary>
        private bool CanRemoveKeyBindingCommandExecute(object p) => p is KeyBindingViewModel;

        /// <summary>Логика выполнения - Удалить действие клавиши</summary>
        private void OnRemoveKeyBindingCommandExecuted(object p) => KeyBindings.Remove((KeyBindingViewModel)p);

        #endregion
    }

    public class KeyBindingViewModel : ViewModel, IEditModel<KeyboardKeyBinding>
    {
        private KeyboardKeyBinding _Model;

        private const int DangerDelay = 300;

        public KeyboardKeyBinding GetModel() => _Model;

        public void SetModel(KeyboardKeyBinding model) => _Model = model;

        public KeyBindingViewModel(KeyboardKeyBinding model)
        {
            _Model = model;
        }

        #region Delay : int - Задержка

        /// <summary>Задержка</summary>
        public int Delay
        {
            get => _Model.Delay;
            set
            {
                if(_Model.Delay == value)
                    return;
                _Model.Delay = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasDelay));
                OnPropertyChanged(nameof(IsDangerDelay));
            }
        }

        #endregion

        #region HasDelay : bool - Задержка установлена

        /// <summary>Задержка установлена</summary>
        public bool HasDelay => Delay > 0;

        #endregion


        #region IsDangerDelay : bool - Долгая задержка


        /// <summary>Долгая задержка</summary>
        public bool IsDangerDelay => Delay > DangerDelay;

        #endregion

        public override string ToString() => _Model.ToString();
    }
}
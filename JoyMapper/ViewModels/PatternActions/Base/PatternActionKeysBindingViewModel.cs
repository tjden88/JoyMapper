using System.Collections.ObjectModel;
using JoyMapper.Models;
using JoyMapper.Views.Windows;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.PatternActions.Base;

/// <summary>
/// Список назначаемых клавиш на то или иное действие
/// </summary>
public class PatternActionKeysBindingViewModel : ViewModel
{

    public PatternActionKeysBindingViewModel(string name) => Name = name;

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
        

    #region Name : string - Имя списка команд

    /// <summary>Имя списка команд</summary>
    public string Name { get; }

    #endregion


    #region Command EditCommand - Редактировать команды

    /// <summary>Редактировать команды</summary>
    private Command _EditCommand;

    /// <summary>Редактировать команды</summary>
    public Command EditCommand => _EditCommand
        ??= new Command(OnEditCommandExecuted, CanEditCommandExecute, "Редактировать команды");

    /// <summary>Проверка возможности выполнения - Редактировать команды</summary>
    private bool CanEditCommandExecute() => true;

    /// <summary>Логика выполнения - Редактировать команды</summary>
    private void OnEditCommandExecuted()
    {
        var wnd = new PatternActionKeyBindingsEdit(KeyBindings, Name)
        {
            Owner = App.ActiveWindow
        };
        if (wnd.ShowDialog() != true) return;

        KeyBindings=new(wnd.ViewModel.KeyBindings);
    }

    #endregion
}
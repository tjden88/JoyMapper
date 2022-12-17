using System.Collections.ObjectModel;
using JoyMapper.Models;
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

}
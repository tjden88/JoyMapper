using System.Collections.ObjectModel;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;
using JoyMapper.Views.Windows;
using WPR.MVVM.Commands.Base;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Секвенции (перебор комбинаций клавиш)
/// </summary>
public class SeqKeySenderViewModel : PatternActionViewModelBase
{
    private const string CollectionName = "Последовательность команд";

    public SeqKeySenderViewModel(SeqKeySenderPatternAction model = null)
    {
        if(model is null)
            return;

        SeqKeysList = new(model.SeqKeysList
            .Select(key => new PatternActionKeysBindingViewModel(CollectionName) {KeyBindings = new(key)}));
    }


    #region SeqKeysList : ObservableCollection<PatternActionKeysBindingViewModel> - Коллекция последовательностей

    /// <summary>Коллекция последовательностей</summary>
    private ObservableCollection<PatternActionKeysBindingViewModel> _SeqKeysList = new();

    /// <summary>Коллекция последовательностей</summary>
    public ObservableCollection<PatternActionKeysBindingViewModel> SeqKeysList
    {
        get => _SeqKeysList;
        set => Set(ref _SeqKeysList, value);
    }

    #endregion


    #region Commands

    #region Command AddSequenceCommand - Добавить последовательность клавиш

    /// <summary>Добавить последовательность клавиш</summary>
    private Command _AddSequenceCommand;

    /// <summary>Добавить последовательность клавиш</summary>
    public Command AddSequenceCommand => _AddSequenceCommand
        ??= new Command(OnAddSequenceCommandExecuted, CanAddSequenceCommandExecute, "Добавить последовательность клавиш");

    /// <summary>Проверка возможности выполнения - Добавить последовательность клавиш</summary>
    private bool CanAddSequenceCommandExecute() => true;

    /// <summary>Логика выполнения - Добавить последовательность клавиш</summary>
    private void OnAddSequenceCommandExecuted()
    {
        var wnd = new PatternActionKeyBindingsEdit(Enumerable.Empty<KeyboardKeyBinding>(), CollectionName)
        {
            Owner = App.ActiveWindow
        };

        if (wnd.ShowDialog() != true || !wnd.ViewModel.KeyBindings.Any())
            return;

        SeqKeysList.Add(new PatternActionKeysBindingViewModel(CollectionName) { KeyBindings = wnd.ViewModel.KeyBindings });
    }

    #endregion


    #region Command EditSequenceCommand - Изменить команды

    /// <summary>Изменить команды</summary>
    private Command _EditSequenceCommand;

    /// <summary>Изменить команды</summary>
    public Command EditSequenceCommand => _EditSequenceCommand
        ??= new Command(OnEditSequenceCommandExecuted, CanEditSequenceCommandExecute, "Изменить команды");

    /// <summary>Проверка возможности выполнения - Изменить команды</summary>
    private bool CanEditSequenceCommandExecute(object p) => p is PatternActionKeysBindingViewModel;

    /// <summary>Логика выполнения - Изменить команды</summary>
    private void OnEditSequenceCommandExecuted(object p)
    {
        var bindings = (PatternActionKeysBindingViewModel)p;
        var wnd = new PatternActionKeyBindingsEdit(bindings.KeyBindings, CollectionName)
        {
            Owner = App.ActiveWindow
        };

        if (wnd.ShowDialog() != true)
            return;

        if (!wnd.ViewModel.KeyBindings.Any())
        {
            SeqKeysList.Remove(bindings);
            return;
        }

        bindings.KeyBindings = new(wnd.ViewModel.KeyBindings);
    }

    #endregion


    #region Command RemoveSequenceCommand - Удалить последовательность

    /// <summary>Удалить последовательность</summary>
    private Command _RemoveSequenceCommand;

    /// <summary>Удалить последовательность</summary>
    public Command RemoveSequenceCommand => _RemoveSequenceCommand
        ??= new Command(OnRemoveSequenceCommandExecuted, CanRemoveSequenceCommandExecute, "Удалить последовательность");

    /// <summary>Проверка возможности выполнения - Удалить последовательность</summary>
    private bool CanRemoveSequenceCommandExecute(object p) => p is PatternActionKeysBindingViewModel;

    /// <summary>Логика выполнения - Удалить последовательность</summary>
    private void OnRemoveSequenceCommandExecuted(object p) => SeqKeysList.Remove((PatternActionKeysBindingViewModel)p);

    #endregion


    #endregion


    public override string Name => "Секвенция";
    public override string Description => "Последовательная отправка команд клавиатуры при каждом активации действия";
    public override PatternActionBase ToModel() =>
        new SeqKeySenderPatternAction
        {
            SeqKeysList = SeqKeysList
                .Select(kb => kb.KeyBindings.ToList())
                .ToList(),
        };

    public override bool IsValid(out string ErrorMessage)
    {
        ErrorMessage = "Клавиатурные команды не назначены";
        return SeqKeysList.Any() && SeqKeysList.All(kb => kb.KeyBindings.Any());
    }
}
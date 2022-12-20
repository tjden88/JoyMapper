using System.Collections.ObjectModel;
using System.Linq;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Секвенции (перебор комбинаций клавиш)
/// </summary>
public class SeqKeySenderViewModel : PatternActionViewModelBase
{

    public SeqKeySenderViewModel(SeqKeySenderPatternAction model)
    {
        if(model is null)
            return;

        SeqKeysList = new(model.SeqKeysList
            .Select(key => 
                new PatternActionKeysBindingViewModel("Последовательность команд")));
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
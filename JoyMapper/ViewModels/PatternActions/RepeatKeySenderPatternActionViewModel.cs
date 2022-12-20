using System.Linq;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Повторные нажатия через интервалы
/// </summary>
public class RepeatKeySenderPatternActionViewModel : PatternActionViewModelBase
{

    public RepeatKeySenderPatternActionViewModel(RepeatKeySenderPatternAction model = null)
    {
        KeyBindings = new("Команды при активации");

        if (model is null)
            return;

        Delay = model.Delay;
        if (model.RepeatCount == 0)
            RepeatWhileActive = true;
        else
        {
            RepeatWhileActive = false;
            RepeatCount = model.RepeatCount;
        }

        KeyBindings.KeyBindings = new(model.KeyBindings);
    }


    #region RepeatCount : int - Количество повторений

    /// <summary>Количество повторений</summary>
    private int _RepeatCount = 4;

    /// <summary>Количество повторений</summary>
    public int RepeatCount
    {
        get => _RepeatCount;
        set => Set(ref _RepeatCount, value);
    }

    #endregion


    #region RepeatWhileActive : bool - Повторять пока активно дейстивие

    /// <summary>Повторять пока активно дейстивие</summary>
    private bool _RepeatWhileActive = true;

    /// <summary>Повторять пока активно дейстивие</summary>
    public bool RepeatWhileActive
    {
        get => _RepeatWhileActive;
        set => Set(ref _RepeatWhileActive, value);
    }

    #endregion

    
    #region Delay : int - Задержка между повторами

    /// <summary>Задержка между повторами</summary>
    private int _Delay = 1000;

    /// <summary>Задержка между повторами</summary>
    public int Delay
    {
        get => _Delay;
        set => Set(ref _Delay, value);
    }

    #endregion


    public PatternActionKeysBindingViewModel KeyBindings { get; set; }


    public override string Name => "Циклическая отправка команд";

    public override string Description => "Повторная отправка клавиатурных команд через назначенные интервалы при активации действия";
    public override PatternActionBase ToModel()
    {
        return new RepeatKeySenderPatternAction
        {
            Delay = Delay,
            RepeatCount = RepeatWhileActive ? 0 : RepeatCount,
            KeyBindings = KeyBindings.KeyBindings.ToList(),
        };
    }

    public override bool IsValid(out string ErrorMessage)
    {
        ErrorMessage = "Клавиатурные команды не назначены";
        return KeyBindings.KeyBindings.Any();
    }
}
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
        if(model is null)
            return;

        Delay = model.Delay;
        RepeatCount = model.RepeatCount;
        KeyBindings = new("Команды при активации")
        {
            KeyBindings = new(model.KeyBindings)
        };
    }


    #region RepeatCount : int - Количество повторений (0 - пока активно действие)

    /// <summary>Количество повторений (0 - пока активно действие)</summary>
    private int _RepeatCount;

    /// <summary>Количество повторений (0 - пока активно действие)</summary>
    public int RepeatCount
    {
        get => _RepeatCount;
        set => Set(ref _RepeatCount, value);
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
            RepeatCount = RepeatCount,
            KeyBindings = KeyBindings.KeyBindings.ToList(),
        };
    }

    public override bool IsValid(out string ErrorMessage)
    {
        ErrorMessage = "Клавиатурные команды не назначены";
        return KeyBindings.KeyBindings.Any();
    }
}
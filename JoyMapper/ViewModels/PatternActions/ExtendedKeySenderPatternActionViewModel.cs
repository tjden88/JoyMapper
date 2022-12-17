using System.Linq;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии, двойном нажатии и удержании
/// </summary>
public class ExtendedKeySenderPatternActionViewModel : PatternActionViewModelBase
{

    public ExtendedKeySenderPatternActionViewModel(ExtendedKeySenderPatternAction Model = null)
    {
        SinglePressKeyBindings = new("Команды однократной активации");

        DoublePressKeyBindings = new("Команды двойной активации");

        LongPressKeyBindings = new("Команды при длительной активации");

        if (Model == null) return;

        if (Model.SinglePressKeyBindings?.Any() == true)
            SinglePressKeyBindings.KeyBindings = new(Model.SinglePressKeyBindings);

        if (Model.DoublePressKeyBindings?.Any() == true)
            DoublePressKeyBindings.KeyBindings = new(Model.DoublePressKeyBindings);

        if (Model.LongPressKeyBindings?.Any() == true)
            LongPressKeyBindings.KeyBindings = new(Model.LongPressKeyBindings);
    }

    public override string Name => "Расширенный триггер";

    public override string Description => "Отправка команд клавиатуры при нажатии, двойном нажатии или удержании кнопки действия или оси";

    public PatternActionKeysBindingViewModel SinglePressKeyBindings { get; set; } 

    public PatternActionKeysBindingViewModel DoublePressKeyBindings { get; set; } 

    public PatternActionKeysBindingViewModel LongPressKeyBindings { get; set; }


    public override PatternActionBase ToModel() =>
        new ExtendedKeySenderPatternAction
        {
            SinglePressKeyBindings = SinglePressKeyBindings.KeyBindings,
            DoublePressKeyBindings = DoublePressKeyBindings.KeyBindings,
            LongPressKeyBindings = LongPressKeyBindings.KeyBindings
        };

    public override bool IsValid(out string ErrorMessage)
    {
        ErrorMessage = "Клавиатурные команды не назначены";
        return SinglePressKeyBindings.KeyBindings.Any() || DoublePressKeyBindings.KeyBindings.Any() || LongPressKeyBindings.KeyBindings.Any();
    }
}
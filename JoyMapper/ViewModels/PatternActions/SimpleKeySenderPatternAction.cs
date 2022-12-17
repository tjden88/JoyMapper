using System.Collections.Generic;
using System.Linq;
using JoyMapper.Models;
using JoyMapper.Models.PatternActions;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.ViewModels.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии и отпускании
/// </summary>
public class SimpleKeySenderPatternActionViewModel : PatternActionViewModelBase
{

    public SimpleKeySenderPatternActionViewModel(SimpleKeySenderPatternAction Model = null)
    {
        PressKeyBindings = new("Команды при активации");
        ReleaseKeyBindings = new("Команды при деактивации");

        if (Model == null) return;

        if (Model.PressKeyBindings?.Any() == true)
            PressKeyBindings.KeyBindings = new(Model.PressKeyBindings);

        if (Model.ReleaseKeyBindings?.Any() == true)
            ReleaseKeyBindings.KeyBindings = new(Model.ReleaseKeyBindings);
    }

    public override string Name => "Простой триггер";

    public override string Description => "Отправка команд клавиатуры при активации и деактивации действия";

    public PatternActionKeysBindingViewModel PressKeyBindings { get; set; }

    public PatternActionKeysBindingViewModel ReleaseKeyBindings { get; set; }


    public override PatternActionBase ToModel() =>
        new SimpleKeySenderPatternAction
        {
            PressKeyBindings = PressKeyBindings.KeyBindings,
            ReleaseKeyBindings = ReleaseKeyBindings.KeyBindings
        };

    public override bool IsValid() => PressKeyBindings.KeyBindings.Any() || ReleaseKeyBindings.KeyBindings.Any();
}
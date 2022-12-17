using System.Collections.Generic;
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
    public SimpleKeySenderPatternActionViewModel() { }

    public SimpleKeySenderPatternActionViewModel(IEnumerable<KeyboardKeyBinding> pressKeyBindings, IEnumerable<KeyboardKeyBinding> releaseKeyBindings)
    {
        PressKeyBindings = new("Команды при активации") {KeyBindings = new(pressKeyBindings)};
        ReleaseKeyBindings = new("Команды при деактивации") {KeyBindings = new(releaseKeyBindings)};
    }

    public override string Name => "Простой триггер";

    public override string Description => "Отправка команд клавиатуры при активации и деактивации действия";

    public PatternActionKeysBindingViewModel PressKeyBindings { get; set; } = new("Команды при активации");

    public PatternActionKeysBindingViewModel ReleaseKeyBindings { get; set; } = new("Команды при деактивации");


    public override PatternActionBase ToModel() =>
        new SimpleKeySenderPatternAction
        {
            PressKeyBindings = PressKeyBindings.KeyBindings,
            ReleaseKeyBindings = ReleaseKeyBindings.KeyBindings
        };
}
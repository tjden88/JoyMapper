using System.Collections.Generic;
using JoyMapper.Models.PatternActions.Base;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии, двойном нажатии и удержании
/// </summary>
public class ExtendedKeySenderPatternAction : PatternActionBase
{
    public override string Name => "Расширенный триггер";

    public override string Description => "Отправка команд клавиатуры при нажатии, двойном нажатии или удержании кнопки действия или оси";

    public List<KeyboardKeyBinding> SinglePressKeyBindings { get; set; } = new();

    public List<KeyboardKeyBinding> DoublePressKeyBindings { get; set; } = new();

    public List<KeyboardKeyBinding> LongPressKeyBindings { get; set; } = new();

}
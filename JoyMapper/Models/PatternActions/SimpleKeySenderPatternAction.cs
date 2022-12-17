using JoyMapper.Models.PatternActions.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии и отпускании
/// </summary>
public class SimpleKeySenderPatternAction : PatternActionBase
{
    public override string Name => "Простой триггер";

    public override string Description => "Отправка команд клавиатуры при активации и деактивации действия";

    public KeyboardKeyBindingGroup PressKeyBindings { get; set; } = new();

    public KeyboardKeyBindingGroup ReleaseKeyBindings { get; set; } = new();

    

}
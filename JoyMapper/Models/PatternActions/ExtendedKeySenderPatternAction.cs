using System.Collections.Generic;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии, двойном нажатии и удержании
/// </summary>
public class ExtendedKeySenderPatternAction : PatternActionBase
{
    public override PatternActionViewModel ToViewModel()
    {
        throw new System.NotImplementedException();
    }

    public List<KeyboardKeyBinding> SinglePressKeyBindings { get; set; } = new();

    public List<KeyboardKeyBinding> DoublePressKeyBindings { get; set; } = new();

    public List<KeyboardKeyBinding> LongPressKeyBindings { get; set; } = new();

}
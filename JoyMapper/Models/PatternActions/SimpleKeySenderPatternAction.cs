using JoyMapper.Models.PatternActions.Base;
using System.Collections.Generic;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии и отпускании
/// </summary>
public class SimpleKeySenderPatternAction : PatternActionBase
{
    public override PatternActionViewModelBase ToViewModel()
    {
        throw new System.NotImplementedException();
    }

    public List<KeyboardKeyBinding> PressKeyBindings { get; set; } = new();

    public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; } = new();

    

}
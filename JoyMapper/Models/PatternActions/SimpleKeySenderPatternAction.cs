using JoyMapper.Models.PatternActions.Base;
using System.Collections.Generic;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии и отпускании
/// </summary>
public class SimpleKeySenderPatternAction : PatternActionBase
{
    public override PatternActionViewModelBase ToViewModel() => 
        new SimpleKeySenderPatternActionViewModel(PressKeyBindings, ReleaseKeyBindings);

    public ICollection<KeyboardKeyBinding> PressKeyBindings { get; set; }

    public ICollection<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }

    

}
using System.Collections.Generic;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии, двойном нажатии и удержании
/// </summary>
public class ExtendedKeySenderPatternAction : PatternActionBase
{
    public override PatternActionViewModelBase ToViewModel() => 
        new ExtendedKeySenderPatternActionViewModel(this);

    public override void BindingStateChanged(bool newState)
    {
        throw new System.NotImplementedException();
    }

    public ICollection<KeyboardKeyBinding> SinglePressKeyBindings { get; set; }

    public ICollection<KeyboardKeyBinding> DoublePressKeyBindings { get; set; } 

    public ICollection<KeyboardKeyBinding> LongPressKeyBindings { get; set; } 

}
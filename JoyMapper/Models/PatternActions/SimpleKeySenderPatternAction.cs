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
        new SimpleKeySenderPatternActionViewModel(this);

    public override void BindingStateChanged(bool newState)
    {
        ReportMessage?.Invoke(newState ? "Нажате кнопки/вход в зону оси" : "Отпускание кнопки/выход из зоны оси");
    }

    public ICollection<KeyboardKeyBinding> PressKeyBindings { get; set; }

    public ICollection<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }

    

}
using System;
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

    protected override void Initialize(IServiceProvider Services)
    {
    }

    protected override void DoReportMode(bool newBindingState)
    {
        ReportMessage?.Invoke(newBindingState ? "Нажате кнопки/вход в зону оси" : "Отпускание кнопки/выход из зоны оси");
    }

    protected override void DoWorkMode(bool newBindingState)
    {
        throw new NotImplementedException();
    }


    public ICollection<KeyboardKeyBinding> PressKeyBindings { get; set; }

    public ICollection<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }

}
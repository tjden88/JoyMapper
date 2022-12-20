using System;
using JoyMapper.Models.PatternActions.Base;
using System.Collections.Generic;
using JoyMapper.Services;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии и отпускании
/// </summary>
public class SimpleKeySenderPatternAction : PatternActionBase
{
    private static KeyboardSender _KeyboardSender;

    public override PatternActionViewModelBase ToViewModel() => 
        new SimpleKeySenderPatternActionViewModel(this);

    protected override void Initialize(IServiceProvider Services)
    {
        if (!LogReportMode)
            _KeyboardSender = Services.GetRequiredService<KeyboardSender>();
    }

    protected override void DoReportMode(bool newBindingState)
    {
        ReportMessage?.Invoke(newBindingState ? "Нажате кнопки/вход в зону оси" : "Отпускание кнопки/выход из зоны оси");
    }

    protected override void DoWorkMode(bool newBindingState)
    {
        _KeyboardSender.SendKeyboardCommands(newBindingState ? PressKeyBindings : ReleaseKeyBindings);
    }


    public List<KeyboardKeyBinding> PressKeyBindings { get; set; }

    public List<KeyboardKeyBinding> ReleaseKeyBindings { get; set; }

}
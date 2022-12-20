using System;
using System.Collections.Generic;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Секвенции (перебор комбинаций клавиш)
/// </summary>
public class SeqKeySenderPatternAction : PatternActionBase
{

    public List<List<KeyboardKeyBinding>> SeqKeysList { get; set; } = new();


    public override PatternActionViewModelBase ToViewModel() => new SeqKeySenderViewModel(this);

    
    protected override void DoReportMode(bool newBindingState)
    {
        if(newBindingState)
            ReportMessage?.Invoke("Отправка команд");
    }


    #region Work

    private int _CurrentIndex;
    private KeyboardSender _KeyboardSender;

    protected override void Initialize(IServiceProvider Services)
    {
        _KeyboardSender = Services.GetRequiredService<KeyboardSender>();
        _CurrentIndex = 0;
    }

    protected override void DoWorkMode(bool newBindingState)
    {
        if(!newBindingState || SeqKeysList.Count == 0) return;

        if (_CurrentIndex == SeqKeysList.Count)
            _CurrentIndex = 0;

        _KeyboardSender.SendKeyboardCommands(SeqKeysList[_CurrentIndex]);
        _CurrentIndex++;
    }
} 
#endregion
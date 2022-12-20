using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Повторные нажатия через интервалы
/// </summary>
public class RepeatKeySenderPatternAction : PatternActionBase
{

    public List<KeyboardKeyBinding> KeyBindings { get; set; }


    /// <summary> Задержка между повторами </summary>
    public int Delay { get; set; }

    /// <summary> Количество повторений (0 - пока активно действие) </summary>
    public int RepeatCount { get; set; }


    public override PatternActionViewModelBase ToViewModel() => new RepeatKeySenderPatternActionViewModel(this);


    protected override void DoReportMode(bool newBindingState)
    {
        if (newBindingState)
            ReportMessage?.Invoke("Начат цикл заданных команд");
    }



    #region Work

    private KeyboardSender _KeyboardSender;

    private int _RepeatIndex;

    private JoyBindingBase _ModificatorBinding;

    private CancellationTokenSource _CancellationTokenSource;

    protected override void Initialize(IServiceProvider Services)
    {
        if (LogReportMode) return;

        _KeyboardSender = Services.GetRequiredService<KeyboardSender>();
        var dataManager = Services.GetRequiredService<DataManager>();
        var parentPattern = dataManager.JoyPatterns.FirstOrDefault(p=> p.PatternAction == this);

        if (parentPattern?.ModificatorId is { } mId)
            _ModificatorBinding = dataManager.Modificators.FirstOrDefault(m => m.Id == mId)?.Binding;

    }

    protected override void DoWorkMode(bool newBindingState)
    {
        if (!newBindingState && RepeatCount == 0)
        {
            _CancellationTokenSource?.Cancel();
            return;
        }

        if (newBindingState)
        {
            _RepeatIndex = 0;
            _CancellationTokenSource?.Cancel();

            var source = new CancellationTokenSource();
            Task.Run(() => SendCommands(source.Token), source.Token);
            _CancellationTokenSource = source;
        }
    }

    private async Task SendCommands(CancellationToken cancel)
    {
        while (!cancel.IsCancellationRequested)
        {
            if (RepeatCount > 0)
            {
                _RepeatIndex++;
                if (_RepeatIndex > RepeatCount)
                {
                    _RepeatIndex = 0;
                    break;
                }
            }

            if(RepeatCount == 0 && _ModificatorBinding?.IsActive == false)
                break;

            _KeyboardSender.SendKeyboardCommands(KeyBindings);
            await Task.Delay(Delay, cancel);
        }

        _CancellationTokenSource.Dispose();
        _CancellationTokenSource = null;
    }

    #endregion
}
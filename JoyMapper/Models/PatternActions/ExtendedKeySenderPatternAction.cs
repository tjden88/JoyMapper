using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Models.PatternActions.Base;
using JoyMapper.Services;
using JoyMapper.Services.Data;
using JoyMapper.ViewModels.PatternActions;
using JoyMapper.ViewModels.PatternActions.Base;
using Microsoft.Extensions.DependencyInjection;

namespace JoyMapper.Models.PatternActions;

/// <summary>
/// Отправка клавиатурных команд при нажатии, двойном нажатии и удержании
/// </summary>
public class ExtendedKeySenderPatternAction : PatternActionBase
{
    public override PatternActionViewModelBase ToViewModel() =>
        new ExtendedKeySenderPatternActionViewModel(this);


    public List<KeyboardKeyBinding> SinglePressKeyBindings { get; set; }

    public List<KeyboardKeyBinding> DoublePressKeyBindings { get; set; }

    public List<KeyboardKeyBinding> LongPressKeyBindings { get; set; }


    #region KeyWatching

    private static KeyboardSender _KeyboardSender;

    protected override void Initialize(IServiceProvider Services)
    {
        var appsett = Services.GetRequiredService<DataManager>();
        _DoublePressDelay = appsett.AppSettings.DoublePressDelay;
        _LongPressDelay = appsett.AppSettings.LongPressDelay;
        _IsDoublePressActionsExist = DoublePressKeyBindings?.Any() ?? false;
        if(!LogReportMode)
            _KeyboardSender = Services.GetRequiredService<KeyboardSender>();
    }


    private bool _IsDoublePressActionsExist; // Существуют ли действия двойного нажатия

    private static int _DoublePressDelay;

    private static int _LongPressDelay;

    private Stopwatch _DelayMeter; // Таймер задержки между нажатиями

    private bool _FirstPressHandled; // Первое нажатие поймано

    private bool _NowPressed;


    protected override void DoReportMode(bool newBindingState) => DoWork(newBindingState, true);

    protected override void DoWorkMode(bool newBindingState) => DoWork(newBindingState, false);

    protected async void DoWork(bool newBindingState, bool ReportMode)
    {
        _NowPressed = newBindingState;

        if(newBindingState && _DelayMeter is null)
            _DelayMeter = Stopwatch.StartNew();

        while (_DelayMeter is not null )
        {

            // Кнопка не нажата и прошло время двойного клика или
            // Кнопка отпущена после первого нажатия и на двойное нажатие действий не назначено
            if (_FirstPressHandled && !newBindingState && (!_IsDoublePressActionsExist && !ReportMode || _DelayMeter?.ElapsedMilliseconds > _DoublePressDelay))
            {
                if (ReportMode)
                    ReportMessage?.Invoke("Одиночное нажатие");
                else
                    _KeyboardSender.SendKeyboardCommands(SinglePressKeyBindings);

                _FirstPressHandled = false;
                _DelayMeter = null;
            }


            if (_NowPressed) // Состояние изменилось на нажатое
            {
                _NowPressed = false;
                if (!_FirstPressHandled) // Регистрируем первое нажатие
                {
                    _FirstPressHandled = true;
                }
                else // Второе нажатие
                {
                    if (ReportMode)
                        ReportMessage?.Invoke("Двойное нажатие");
                    else
                        _KeyboardSender.SendKeyboardCommands(DoublePressKeyBindings);

                    _FirstPressHandled = false;
                    _DelayMeter = null;
                }
            }

            // Кнопка нажата больше времени долгого нажатия
            if (_FirstPressHandled && newBindingState && _DelayMeter?.ElapsedMilliseconds > _LongPressDelay)
            {
                if (ReportMode)
                    ReportMessage?.Invoke("Долгое нажатие");
                else
                    _KeyboardSender.SendKeyboardCommands(LongPressKeyBindings);

                _FirstPressHandled = false;
                _DelayMeter = null;
            }
            await Task.Delay(20);
        }
    }


    #endregion

}
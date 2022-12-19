using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Models.PatternActions.Base;
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



    public ICollection<KeyboardKeyBinding> SinglePressKeyBindings { get; set; }

    public ICollection<KeyboardKeyBinding> DoublePressKeyBindings { get; set; }

    public ICollection<KeyboardKeyBinding> LongPressKeyBindings { get; set; }


    #region KeyWatching

    protected override void Initialize(IServiceProvider Services)
    {
        var appsett = Services.GetRequiredService<DataManager>();
        _DoublePressDelay = appsett.AppSettings.DoublePressDelay;
        _LongPressDelay = appsett.AppSettings.LongPressDelay;
        _IsDoublePressActionsExist = DoublePressKeyBindings?.Any() ?? false;
    }

    protected override void DoWorkMode(bool newBindingState)
    {
        throw new NotImplementedException();
    }

    private bool _IsDoublePressActionsExist; // Существуют ли действия двойного нажатия

    private static int _DoublePressDelay;

    private static int _LongPressDelay;


    private Stopwatch _DelayMeter; // Таймер задержки между нажатиями

    private bool _FirstPressHandled; // Первое нажатие поймано


    private bool _NowPressed;

    protected override async void DoReportMode(bool newBindingState)
    {
        _NowPressed = newBindingState;

        if(newBindingState && _DelayMeter is null)
            _DelayMeter = Stopwatch.StartNew();

        while (_DelayMeter is not null )
        {

            // Кнопка не нажата и прошло время двойного клика или
            // Кнопка отпущена после первого нажатия и на двойное нажатие действий не назначено
            if (_FirstPressHandled && !newBindingState && (!_IsDoublePressActionsExist && !LogReportMode || _DelayMeter?.ElapsedMilliseconds > _DoublePressDelay))
            {
                ReportMessage?.Invoke("Одиночное нажатие");

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
                    ReportMessage?.Invoke("Двойное нажатие");
                    _FirstPressHandled = false;
                    _DelayMeter = null;
                }
            }

            // Кнопка нажата больше времени долгого нажатия
            if (_FirstPressHandled && newBindingState && _DelayMeter?.ElapsedMilliseconds > _LongPressDelay)
            {
                ReportMessage?.Invoke("Долгое нажатие");
                _FirstPressHandled = false;
                _DelayMeter = null;
            }
            await Task.Delay(20);
        }
    }


    #endregion

}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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

    public override void Initialize(IServiceProvider Services)
    {
        var appsett = Services.GetRequiredService<DataManager>();
        _DoublePressDelay = appsett.AppSettings.DoublePressDelay;
        _LongPressDelay = appsett.AppSettings.LongPressDelay;
        _IsDoublePressActionsExist = DoublePressKeyBindings?.Any() ?? false;
    }

    public ICollection<KeyboardKeyBinding> SinglePressKeyBindings { get; set; }

    public ICollection<KeyboardKeyBinding> DoublePressKeyBindings { get; set; } 

    public ICollection<KeyboardKeyBinding> LongPressKeyBindings { get; set; }



    #region KeyWatching

    private bool _IsDoublePressActionsExist; // Существуют ли действия двойного нажатия

    private static int _DoublePressDelay;

    private static int _LongPressDelay;

    private Stopwatch _DelayMeter; // Таймер задержки между нажатиями

    private bool _FirstPressHandled; // Первое нажатие поймано

    // Оптимизировать одиночное нажатие, если для двойного не назначено действий
    public bool OptimizeSingleClick { get; set; } = true;


    public override void BindingStateChanged(bool newState)
    {
        
        // Кнопка не нажата и прошло время двойного клика или
        // Кнопка отпущена после первого нажатия и на двойное нажатие действий не назначено
        if (_FirstPressHandled && !newState && (!_IsDoublePressActionsExist && OptimizeSingleClick || _DelayMeter?.ElapsedMilliseconds > _DoublePressDelay))
        {
            ReportMessage?.Invoke("Одиночное нажатие");

            Debug.WriteLine("SinglePressSend: ");
            _FirstPressHandled = false;
            _DelayMeter = null;
        }


        if (newState) // Состояние изменилось на нажатое
        {
            if (!_FirstPressHandled) // Регистрируем первое нажатие
            {
                _FirstPressHandled = true;
                _DelayMeter = Stopwatch.StartNew();
            }
            else // Второе нажатие
            {
                ReportMessage?.Invoke("Двойное нажатие");
                Debug.WriteLine("DoublePressSend: ");
                _FirstPressHandled = false;
                _DelayMeter = null;
            }
        }

        // Кнопка нажата больше времени долгого нажатия
        if (_FirstPressHandled && newState && _DelayMeter?.ElapsedMilliseconds > _LongPressDelay)
        {
            ReportMessage?.Invoke("Долгое нажатие");
            Debug.WriteLine("LongPressSend: ");
            _FirstPressHandled = false;
            _DelayMeter = null;
        }
    }


    #endregion

}
using System;
using System.ComponentModel.DataAnnotations.Schema;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.Models.PatternActions.Base;


/// <summary>
/// Базовый класс для возможных действий паттернов
/// </summary>
public abstract class PatternActionBase
{
    protected bool LogReportMode;
    public abstract PatternActionViewModelBase ToViewModel();


    /// <summary> Инициализировать зависимости для начала работы </summary>
    protected abstract void Initialize(IServiceProvider Services);

    /// <summary> Отработка в режиме отправки статусов </summary>
    protected abstract void DoReportMode(bool newBindingState);

    /// <summary> Отработка в "боевом" режиме </summary>
    protected abstract void DoWorkMode(bool newBindingState);

    /// <summary>
    /// Проинициализировать необходимые данные для запуска отслеживания состояний.
    /// LogReportsMode - true, когда необходимо только вызвать отчёт об изменении действия
    /// </summary>
    public void Initialize(IServiceProvider Services, bool LogReportsMode)
    {
        LogReportMode = LogReportsMode;
        Initialize(Services);
    }

    /// <summary>
    /// Сообщает, что привязка кнопки изменила своё состояние.
    /// Отрабатывает это событие
    /// </summary>
    /// <param name="newState">Новое состояние кнопки или оси</param>
    public void BindingStateChanged(bool newState)
    {
        if(LogReportMode)
            DoReportMode(newState);
        else
            DoWorkMode(newState);
    }

    /// <summary>
    /// Вызывает действие, сообщающее, какое действие было совершено
    /// </summary>
    [NotMapped]
    public Action<string> ReportMessage { get; set; }
}
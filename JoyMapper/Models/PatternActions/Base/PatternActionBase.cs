using System;
using JoyMapper.ViewModels.PatternActions.Base;

namespace JoyMapper.Models.PatternActions.Base;


/// <summary>
/// Базовый класс для возможных действий паттернов
/// </summary>
public abstract class PatternActionBase 
{
    public abstract PatternActionViewModelBase ToViewModel();


    /// <summary>
    /// Проинициализировать необходимые данные для запуска отслеживания состояний.
    /// LogReports - true, когда необходимо только вызвать отчёт об изменении действия
    /// </summary>
    public abstract void Initialize(IServiceProvider Services, bool LogReports);

    /// <summary>
    /// Сообщает, что привязка кнопки изменила своё состояние
    /// </summary>
    /// <param name="newState">Новое состояние кнопки или оси</param>
    public abstract void BindingStateChanged(bool newState);

    /// <summary>
    /// Вызывает действие, сообщающее, какое действие было совершено
    /// </summary>
    public Action<string> ReportMessage { get; set; }
}
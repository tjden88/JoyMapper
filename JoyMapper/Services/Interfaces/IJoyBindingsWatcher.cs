using System.Collections.Generic;
using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Services.Interfaces;

/// <summary>
/// Отслеживание изменений назначенных действий джойстика
/// </summary>
public interface IJoyBindingsWatcher
{
    /// <summary> Начать отслеживание выбранных привязок </summary>
    void StartWatching(IEnumerable<JoyBindingBase> bindings);


    /// <summary> Получить изменения привязок с предыдущего опроса </summary>
    ICollection<JoyBindingBase> GetChanges();


    /// <summary> Остановить отслеживание, очистить данные </summary>
    void StopWatching();
}
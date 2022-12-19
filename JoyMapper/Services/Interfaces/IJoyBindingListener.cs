using System;
using System.Collections.Generic;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Models.Listeners;

namespace JoyMapper.Services.Interfaces;

/// <summary>
/// Прослушивание состояний джойстика
/// </summary>
public interface IJoyBindingListener
{
    /// <summary> Вызывается, когда получены изменения </summary>
    public Action<IEnumerable<JoyBindingBase>> ChangesHandled { get; set; }


    /// <summary> Начать прослушивание выбранных привязок без учёта модификаторов </summary>
    void StartListen(IEnumerable<JoyBindingBase> bindings);

    /// <summary> Начать прослушивание выбранных привязок с учётом модификаторов</summary>
    void StartListen(IEnumerable<ModificatedJoyBinding> bindings);


    /// <summary> Остановить прослушивание </summary>
    void StopListen();
}
using System;
using System.Collections.Generic;
using JoyMapper.Models.JoyBindings.Base;

namespace JoyMapper.Services.Interfaces;

/// <summary>
/// Прослушивание состояний джойстика
/// </summary>
public interface IJoyBindingListener
{
    /// <summary> Вызывается, когда получены изменения </summary>
    public Action<IEnumerable<JoyBindingBase>> ChangesHandled { get; set; }


    /// <summary> Начать прослушивание выбранных привязок </summary>
    void StartListen(IEnumerable<JoyBindingBase> bindings);


    /// <summary> Остановить прослушивание </summary>
    void StopListen();
}
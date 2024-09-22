using System.Collections.Generic;
using JoyMapper.Models;

namespace JoyMapper.Services.Interfaces;

/// <summary>
/// Управление подключёнными джойстиками и опрос их состояния
/// </summary>
public interface IJoystickStateManager
{
    /// <summary> Все подключённые устройства </summary>
    IEnumerable<JoystickData> GetConnectedJoysticks();

    /// <summary> Подключиться к необходимым джойстикам </summary>
    void AcquireJoysticks(IEnumerable<JoystickData> Joysticks);

    /// <summary> Получить изменения состояний подключённых джойстиков </summary>
    IEnumerable<JoyStateData> GetJoyStateChanges(); 
}
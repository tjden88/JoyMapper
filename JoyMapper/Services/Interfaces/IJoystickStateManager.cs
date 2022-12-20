using System.Collections.Generic;
using JoyMapper.Models;

namespace JoyMapper.Services.Interfaces;

/// <summary>
/// Управление подключёнными джойстиками и опрос их состояния
/// </summary>
public interface IJoystickStateManager
{
    /// <summary> Все подключённые устройства </summary>
    IEnumerable<string> GetConnectedJoysticks();


    /// <summary> Получить актуальное состояние джойстика </summary>
    JoyState GetJoyState(string joystickName); 
}
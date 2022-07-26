﻿using System.Collections.Generic;
using JoyMapper.Models;

namespace JoyMapper.Services.Interfaces;

/// <summary>
/// Отслеживание изменений в паттернах
/// </summary>
public interface IJoyPatternListener
{
    /// <summary> Начать отслеживание выбранных паттернов в автоматическом режиме </summary>
    void StartWatching(ICollection<JoyPattern> Patterns);

    /// <summary> Остановить отслеживание, очистить данные </summary>
    void StopWatching();

}
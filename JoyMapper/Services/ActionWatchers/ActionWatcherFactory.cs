using System;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.Services.ActionWatchers
{
    /// <summary>
    /// Фабрика - создание отслеживателей действий
    /// </summary>
    internal static class ActionWatcherFactory
    {
        public static ActionWatcherBase CreateActionWatcherBase(JoyActionBase JoyAction) =>
            JoyAction switch
            {
                AxisJoyAction axisJoyAction => new AxisActionWatcher(axisJoyAction),
                SimpleButtonJoyAction simpleButtonJoyAction => new SimpleButtonActionWatcher(simpleButtonJoyAction),
                ExtendedButtonJoyAction extendedButtonJoyAction => new ExtendedButtonActionWatcher(extendedButtonJoyAction),
                _ => throw new NotSupportedException("Неизвестный тип действия джойстика")
            };
    }
}

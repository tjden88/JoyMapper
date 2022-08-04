using System;
using System.Collections.Generic;
using JoyMapper.Models;

namespace JoyMapper.Services.ActionWatchers
{
    /// <summary>
    /// Следит за состоянием действия джойстика. Базовый класс.
    /// Отправляет команды клавиатуры в очередь при возникновении изменений состояния
    /// </summary>
    internal abstract class ActionWatcherBase
    {
        /// <summary> Обновить состояние действия </summary>
        public abstract void Poll(JoyState joyState);


        /// <summary> Активно ли состояние действия </summary>
        protected bool IsActive { get; set; }

        /// <summary> Отправить клавиатурные команды в очередь команд </summary>
        protected void SendKeyboardCommands(List<KeyboardKeyBinding> keyboardKeyBindings)
        {
            //throw new NotImplementedException();
        }
    }
}

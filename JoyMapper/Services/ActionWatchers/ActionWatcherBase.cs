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

        /// <summary> Разрешить отправку команд клавиатуры </summary>
        public bool AlloySendKeyboardCommands { get; set; } = true;


        /// <summary> Обновить состояние действия </summary>
        public abstract void Poll(JoyState joyState);


        /// <summary> Активно ли состояние действия </summary>
        public bool IsActive { get; set; }

        /// <summary> Отправить клавиатурные команды в очередь команд </summary>
        protected void SendKeyboardCommands(List<KeyboardKeyBinding> keyboardKeyBindings)
        {
            if(!AlloySendKeyboardCommands) return;
            throw new NotImplementedException();
        }
    }
}

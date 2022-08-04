using System;
using System.Collections.Generic;
using JoyMapper.Models;
using JoyMapper.Models.JoyActions;

namespace JoyMapper.Services.ActionWatchers
{
    /// <summary>
    /// Следит за состоянием действия джойстика. Базовый класс.
    /// Отправляет команды клавиатуры в очередь при возникновении изменений состояния
    /// </summary>
    internal abstract class ActionWatcherBase
    {

        /// <summary> Ассоциированное действие </summary>
        public abstract JoyActionBase JoyAction { get; }


        /// <summary> Обновить состояние действия </summary>
        public abstract void Poll(JoyState joyState, bool SendCommands);


        /// <summary> Активно ли состояние действия </summary>
        public bool IsActive { get; set; }

        /// <summary> Отправить клавиатурные команды в очередь команд </summary>
        protected void SendKeyboardCommands(List<KeyboardKeyBinding> keyboardKeyBindings)
        {
            throw new NotImplementedException();
        }
    }
}

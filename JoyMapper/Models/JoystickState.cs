using System.Collections.Generic;

namespace JoyMapper.Models
{
    /// <summary>
    /// Текущий статус отслеживаемого джойстика
    /// </summary>
    internal class JoystickState
    {
        public struct BtnState
        {
            public int BtnNumber { get; set; }

            public bool IsPressed { get; set; }
        }

        /// <summary> Имя джойстика </summary>
        public string JoystickName { get; set; }

        /// <summary> Статус используемых кнопок </summary>
        public List<BtnState> BtnStates { get; set; } = new();

    }
}

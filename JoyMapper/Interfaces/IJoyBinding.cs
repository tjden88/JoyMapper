using JoyMapper.Models;

namespace JoyMapper.Interfaces
{
    /// <summary>
    /// Выбранная кнопка или ось джойстика
    /// </summary>
    internal interface IJoyBinding
    {
        /// <summary>
        /// Имя привязанного джойстика
        /// </summary>
        string JoyName { get; }


        /// <summary>
        /// Активно ли действие в текущий момент
        /// </summary>
        /// <param name="joyState">Статус джойстика</param>
        /// <returns></returns>
        bool IsActive(JoyState joyState);


        /// <summary>
        /// Описание действия
        /// </summary>
        string Description { get; }
    }
}

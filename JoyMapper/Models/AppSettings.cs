namespace JoyMapper.Models
{
    /// <summary>
    /// Настройки программы
    /// </summary>
    internal class AppSettings
    {
        /// <summary> Интервал опроса джойстиков </summary>
        public int JoystickPollingDelay { get; set; } = 50;

        /// <summary> Интервал задержки между нажатиями кнопок клавиатуры </summary>
        public int KeyboardInputDelay { get; set; } = 5;
    }
}

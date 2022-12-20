namespace JoyMapper.Models;

/// <summary>
/// Настройки программы
/// </summary>
public class AppSettings
{
    public string AppVersion { get; set; }


    /// <summary> Интервал опроса джойстиков </summary>
    public int JoystickPollingDelay { get; set; } = 50;

    /// <summary> Задержка двойного нажатия кнопки </summary>
    public int DoublePressDelay { get; set; } = 400;

    /// <summary> Задержка долгого нажатия кнопки </summary>
    public int LongPressDelay { get; set; } = 500;

    /// <summary> Id текущей цветовой схемы </summary>
    public int CurrentColorCheme { get; set; }
}
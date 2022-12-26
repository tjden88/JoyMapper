using System.Windows.Media;
using WPR.ColorTheme;

namespace JoyMapper.Models;

/// <summary>
/// Цветовая схема программы
/// </summary>
public class ColorTheme
{
    public int Id { get; set; }

    public Color PrimaryColor { get; set; }

    public Color AccentColor { get; set; }

    /// <summary> Установить тему </summary>
    public void SetTheme()
    {
        StyleHelper.SetPrimaryColor(PrimaryColor);
        StyleHelper.SetAccentColor(AccentColor);
    }
}
using System.Windows.Media;
using WPR;

namespace JoyMapper.Models
{
    /// <summary>
    /// Цветовая схема программы
    /// </summary>
    internal class ColorTheme
    {
        public int Id { get; set; }

        public Color PrimaryColor { get; set; }

        public Color AccentColor { get; set; }

        /// <summary> Установить тему </summary>
        public void SetTheme()
        {
            Design.SetPrimaryColor(PrimaryColor);
            Design.SetAccentColor(AccentColor);
        }
    }
}

using System.Linq;
using System.Windows;
using JoyMapper.Models;

namespace JoyMapper.Services
{
    /// <summary>
    /// Сервис работы с паттернами
    /// </summary>
    internal class PatternService
    {
        /// <summary>
        /// Добавить новый паттерн
        /// </summary>
        /// <returns>null, если пользователь отказался</returns>
        public KeyPattern AddPattern()
        {
            var wnd = new AddPattern()
            {
                Owner = Application.Current.Windows.Cast<Window>().First(w => w.IsActive),
                Title = "Добавить паттерн"
            };
            if (wnd.ShowDialog() != true) return null;

            var pattern = new KeyPattern
            {
                JoyKey = wnd.JoyButton,
                JoyName = wnd.JoyName,
                PressKeyBindings = wnd.PressKeyBindings.ToList(),
                ReleaseKeyBindings = wnd.ReleaseKeyBindings.ToList(),
                Name = wnd.PatternName,
            };
            App.DataManager.AddKeyPattern(pattern);
            return pattern;
        }
    }
}

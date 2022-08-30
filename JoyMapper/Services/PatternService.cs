using JoyMapper.Helpers;
using JoyMapper.Models;
using JoyMapper.ViewModels;
using JoyMapper.Views;

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
           using var vm = new EditPatternWindowViewModel
           {
                Title = "Добавить паттерн"
            };
            var wnd = new EditPatternWindow
            {
                Owner = App.ActiveWindow,
                DataContext = vm
            };
            if (wnd.ShowDialog() != true) return null;

            var pattern = BuildPattern(vm);
            App.DataManager.AddKeyPattern(pattern);
            return pattern;
        }

        /// <summary>
        /// Редактирование существующего паттерна
        /// </summary>
        /// <param name="pattern">Редактируемый паттерн</param>
        /// <returns>null, если пользователь отказался</returns>
        public KeyPattern EditPattern(KeyPattern pattern)
        {
            using var vm = new EditPatternWindowViewModel(pattern.JoyAction.ToViewModel())
            {
                Title = $"Редактировать паттерн: {pattern.Name}",
                JoyName = pattern.JoyName,
                PatternName = pattern.Name,
            };
            var wnd = new EditPatternWindow
            {
                Owner = App.ActiveWindow,
                DataContext = vm
            };
            if (wnd.ShowDialog() != true) return null;

            var editPattern = BuildPattern(vm);
            editPattern.Id = pattern.Id;
            App.DataManager.UpdateKeyPattern(editPattern);
            return editPattern;
        }

        private KeyPattern BuildPattern(EditPatternWindowViewModel windowViewModel)
        {
            var keyPattern = new KeyPattern
            {
                JoyAction = windowViewModel.JoyAction.ToModel(),
                JoyName = windowViewModel.JoyName,
                Name = windowViewModel.PatternName,
            };
            return keyPattern;
        }
    }
}

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
           var vm = new AddPatternViewModel
           {
                Title = "Добавить паттерн"
            };
            var wnd = new AddPattern
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
            var vm = new AddPatternViewModel(pattern.JoyAction.ToViewModel())
            {
                Title = $"Редактировать паттерн: {pattern.Name}",
                JoyName = pattern.JoyName,
                PatternName = pattern.Name,
            };
            var wnd = new AddPattern
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

        private KeyPattern BuildPattern(AddPatternViewModel ViewModel)
        {
            var keyPattern = new KeyPattern
            {
                JoyAction = ViewModel.JoyAction.ToModel(),
                JoyName = ViewModel.JoyName,
                Name = ViewModel.PatternName,
            };
            return keyPattern;
        }
    }
}

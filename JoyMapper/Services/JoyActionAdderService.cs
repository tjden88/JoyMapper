using JoyMapper.Models;
using JoyMapper.Models.JoyActions;
using JoyMapper.ViewModels;
using JoyMapper.Views;

namespace JoyMapper.Services
{
    /// <summary>
    /// Добавляет привязку действия джойстика
    /// </summary>
    internal class JoyActionAdderService
    {
        /// <summary> Получить привязку к действию джойстика </summary>
        public JoyActionBase MapJoyAction(out string JoyName)
        {
            using var vm = new AddJoyActionViewModel();
            var wnd = new AddJoyAction()
            {
                Owner = App.ActiveWindow,
                DataContext = vm,
            };

            var mapJoyAction = wnd.ShowDialog() != true ? null : vm.JoyAction;
            JoyName = vm.JoyName;
            return mapJoyAction;
        }

        /// <summary> Получить привязку кнопки джойстика </summary>
        public JoyButton MapJoyButton(out string JoyName)
        {
            using var vm = new AddJoyActionViewModel {IsOnlyButtons = true};
            var wnd = new AddJoyAction
            {
                Owner = App.ActiveWindow,
                DataContext = vm,
            };

            var button = wnd.ShowDialog() != true 
                ? null 
                : vm.JoyAction is SimpleButtonJoyAction sbj
                ? sbj.Button
                : null;

            JoyName = vm.JoyName;
            return button;
        }
    }
}

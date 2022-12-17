using System;
using System.Windows;
using JoyMapper.ViewModels.Windows;
using WPR;

namespace JoyMapper.Views.Windows
{
    /// <summary>
    /// Логика взаимодействия для EditPatternWindow.xaml
    /// </summary>
    public partial class EditPatternWindow : Window
    {
        public EditPatternViewModel ViewModel { get; }

        public EditPatternWindow(EditPatternViewModel ViewModel)
        {
            this.ViewModel = ViewModel;
            InitializeComponent();
        }


        private void EditPatternWindow_OnLoaded(object Sender, RoutedEventArgs E)
        {
            if(ViewModel.JoyBinding == null)
                ViewModel.AttachJoyButtonCommand.Execute();
        }

        private void EditPatternWindow_OnClosed(object Sender, EventArgs E)
        {
            ViewModel.Dispose();
        }

        private async void ButtonSave_OnClick(object Sender, RoutedEventArgs E)
        {
            var vm = ViewModel;

            if (vm.JoyBinding == null)
            {
                await WPRMessageBox.InformationAsync(this, "Не определена кнопка или ось контроллера для назначения паттерна");
                return;
            }

            if (vm.PatternActionView.ViewModel.SelectedPatternAction == null)
            {
                await WPRMessageBox.InformationAsync(this, "Не выбрано действие паттерна");
                return;
            }

            if (!vm.PatternActionView.ViewModel.SelectedPatternAction.IsValid(out var errorMessage))
            {
                await WPRMessageBox.InformationAsync(this, errorMessage);
                return;
            }

            var patternName = vm.PatternName?.Trim();

            if (string.IsNullOrEmpty(patternName))
            {
                await WPRMessageBox.InformationAsync(this, "Введите имя паттерна");
                return;
            }

            vm.PatternName = patternName;
            DialogResult = true;
        }
    }
}

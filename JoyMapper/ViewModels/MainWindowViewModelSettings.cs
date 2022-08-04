using System.Collections.Generic;
using System.Diagnostics;
using System.Windows.Media;
using JoyMapper.Models;
using WPR.MVVM.Commands;

namespace JoyMapper.ViewModels
{
    internal partial class MainWindowViewModel
    {
        public string AppVersion => App.AppVersion;

        private const string DonateLink = "https://www.tinkoff.ru/rm/dultsev.denis1/G2APs2254";

        private const string HomeUrl = "https://github.com/tjden88/JoyMapper";

        private const string ReportProblemAddress = "https://github.com/tjden88/JoyMapper/issues";


        #region Props

        #region JoystickPollingDelay : int - Интервал опроса джойстиков

        /// <summary>Интервал опроса джойстиков</summary>
        public int JoystickPollingDelay
        {
            get => App.DataManager.AppSettings.JoystickPollingDelay;
            set
            {
                if(Equals(App.DataManager.AppSettings.JoystickPollingDelay, value)) return;
                App.DataManager.AppSettings.JoystickPollingDelay = value;
                App.DataManager.SaveData();
                OnPropertyChanged(nameof(JoystickPollingDelay));
            }
        }

        #endregion

        #region KeyboardDelay : int - Интервал команд клавиатуры

        /// <summary>Интервал команд клавиатуры</summary>
        public int KeyboardDelay
        {
            get => App.DataManager.AppSettings.KeyboardInputDelay;
            set
            {
                if (Equals(App.DataManager.AppSettings.KeyboardInputDelay, value)) return;
                App.DataManager.AppSettings.KeyboardInputDelay = value;
                App.DataManager.SaveData();
                OnPropertyChanged(nameof(KeyboardDelay));
            }
        }

        #endregion

        #region CurrentColorTheme : ColorTheme - Текущая цветовая схема

        /// <summary>Текущая цветовая схема</summary>
        private ColorTheme _CurrentColorTheme;

        /// <summary>Текущая цветовая схема</summary>
        public ColorTheme CurrentColorTheme
        {
            get => _CurrentColorTheme;
            set => IfSet(ref _CurrentColorTheme, value).Then(v =>
            {
                v.SetTheme();
                App.DataManager.AppSettings.CurrentColorCheme = v.Id;
                App.DataManager.SaveData();
            });
        }

        #endregion

        public IEnumerable<ColorTheme> ColorThemes => new List<ColorTheme>()
        {
            new ()
            {
                Id = 0,
                PrimaryColor = (Color) ColorConverter.ConvertFromString("#3F51B5")!,
                AccentColor = (Color) ColorConverter.ConvertFromString("#FF5722")!,
            }, 
            new ()
            {
                Id = 1,
                PrimaryColor = (Color) ColorConverter.ConvertFromString("#00838f")!,
                AccentColor = (Color) ColorConverter.ConvertFromString("#757575")!,
            },
            new ()
            {
                Id = 2,
                PrimaryColor = (Color) ColorConverter.ConvertFromString("#757575")!,
                AccentColor = (Color) ColorConverter.ConvertFromString("#26c6da")!,
            },
            new ()
            {
                Id = 3,
                PrimaryColor = (Color) ColorConverter.ConvertFromString("#5d4037")!,
                AccentColor = (Color) ColorConverter.ConvertFromString("#81c784")!,
            },

        };

        #endregion


        #region Command GoToHomepageCommand - Перейти на страницу проекта

        /// <summary>Перейти на страницу проекта</summary>
        private Command _GoToHomepageCommand;

        /// <summary>Перейти на страницу проекта</summary>
        public Command GoToHomepageCommand => _GoToHomepageCommand
            ??= new Command(OnGoToHomepageCommandExecuted, CanGoToHomepageCommandExecute, "Перейти на страницу проекта");

        /// <summary>Проверка возможности выполнения - Перейти на страницу проекта</summary>
        private bool CanGoToHomepageCommandExecute() => true;

        /// <summary>Логика выполнения - Перейти на страницу проекта</summary>
        private void OnGoToHomepageCommandExecuted() => OpenWebPage(HomeUrl);

        #endregion

        #region Command ReportProblemCommand - Сообщить о проблеме

        /// <summary>Сообщить о проблеме</summary>
        private Command _ReportProblemCommand;

        /// <summary>Сообщить о проблеме</summary>
        public Command ReportProblemCommand => _ReportProblemCommand
            ??= new Command(OnReportProblemCommandExecuted, CanReportProblemCommandExecute, "Сообщить о проблеме");

        /// <summary>Проверка возможности выполнения - Сообщить о проблеме</summary>
        private bool CanReportProblemCommandExecute() => true;

        /// <summary>Логика выполнения - Сообщить о проблеме</summary>
        private void OnReportProblemCommandExecuted() => OpenWebPage(ReportProblemAddress);

        #endregion

        #region Command MakeDonateCommand - Поддержать разработчика

        /// <summary>Поддержать разработчика</summary>
        private Command _MakeDonateCommand;

        /// <summary>Поддержать разработчика</summary>
        public Command MakeDonateCommand => _MakeDonateCommand
            ??= new Command(OnMakeDonateCommandExecuted, CanMakeDonateCommandExecute, "Поддержать разработчика");

        /// <summary>Проверка возможности выполнения - Поддержать разработчика</summary>
        private bool CanMakeDonateCommandExecute() => true;

        /// <summary>Логика выполнения - Поддержать разработчика</summary>
        private void OnMakeDonateCommandExecuted() => OpenWebPage(DonateLink);

        #endregion

        private void OpenWebPage(string Address)
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = Address,
                UseShellExecute = true
            });
        }



    }
}

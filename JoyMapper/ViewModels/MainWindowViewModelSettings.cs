using System.Diagnostics;
using WPR.MVVM.Commands;

namespace JoyMapper.ViewModels
{
    internal partial class MainWindowViewModel
    {
        public string AppVersion => App.AppVersion;

        private const string DonateLink = "https://www.tinkoff.ru/rm/dultsev.denis1/G2APs2254";

        private const string HomeUrl = "https://github.com/tjden88/JoyMapper";

        private const string ReportProblemAddress = "https://github.com/tjden88/JoyMapper/issues";

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

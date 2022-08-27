using System.Diagnostics;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class UpdateWindowViewModel : ViewModel
    {
        #region ReleaseNotes : string - Заметки о новой версии

        /// <summary>Заметки о новой версии</summary>
        private string _ReleaseNotes;

        /// <summary>Заметки о новой версии</summary>
        public string ReleaseNotes
        {
            get => _ReleaseNotes;
            set => Set(ref _ReleaseNotes, value);
        }

        #endregion

        #region DownloadLink : string - Ссылка на скачивание

        /// <summary>Ссылка на скачивание</summary>
        private string _DownloadLink;

        /// <summary>Ссылка на скачивание</summary>
        public string DownloadLink
        {
            get => _DownloadLink;
            set => Set(ref _DownloadLink, value);
        }

        #endregion

        #region Command GoToDownloadLinkCommand - Перейти на страницу загрузки обновления

        /// <summary>Перейти на страницу загрузки обновления</summary>
        private Command _GoToDownloadLinkCommand;

        /// <summary>Перейти на страницу загрузки обновления</summary>
        public Command GoToDownloadLinkCommand => _GoToDownloadLinkCommand
            ??= new Command(OnGoToDownloadLinkCommandExecuted, CanGoToDownloadLinkCommandExecute, "Перейти на страницу загрузки обновления");

        /// <summary>Проверка возможности выполнения - Перейти на страницу загрузки обновления</summary>
        private bool CanGoToDownloadLinkCommandExecute() => DownloadLink != null;

        /// <summary>Логика выполнения - Перейти на страницу загрузки обновления</summary>
        private void OnGoToDownloadLinkCommandExecuted()
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = DownloadLink,
                UseShellExecute = true
            });
        }

        #endregion
    }
}

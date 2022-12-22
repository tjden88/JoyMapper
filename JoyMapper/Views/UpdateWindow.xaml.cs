using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Views;

/// <summary>
/// Логика взаимодействия для UpdateWindow.xaml
/// </summary>
public partial class UpdateWindow : Window
{
    public UpdateWindowViewModel ViewModel { get; }

    public UpdateWindow(UpdateWindowViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = ViewModel;
        InitializeComponent();
    }


    public class UpdateWindowViewModel : ViewModel
    {
        #region LastUpdateReleaseNotes : string - Заметки о новой версии

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
        private bool CanGoToDownloadLinkCommandExecute() => !string.IsNullOrEmpty(DownloadLink);

        /// <summary>Логика выполнения - Перейти на страницу загрузки обновления</summary>
        private void OnGoToDownloadLinkCommandExecuted()
        {
            var downloaderFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JoyMapperUpdater.exe");

            if (!File.Exists(downloaderFile))
            {
                try
                {
                    var res = Properties.Resources.JoyMapperUpdater;
                    File.WriteAllBytes(downloaderFile, res);
                }
                catch (Exception e)
                {
                    Debug.WriteLine(e);
                }
            }


            if (File.Exists(downloaderFile))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = downloaderFile,
                    UseShellExecute = true,
                    Arguments = "1000", // Задержка
                });
            }
            else
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = DownloadLink,
                    UseShellExecute = true,
                });
            }

            Application.Current.Shutdown();
        }

        #endregion
    }
}
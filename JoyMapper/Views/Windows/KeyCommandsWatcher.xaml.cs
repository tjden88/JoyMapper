using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows;
using JoyMapper.Services;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Views.Windows;

/// <summary>
/// Логика взаимодействия для KeyCommandsWatcher.xaml
/// </summary>
public partial class KeyCommandsWatcher : Window
{
    public KeyCommandsWatcherViewModel ViewModel { get; }

    public KeyCommandsWatcher(KeyCommandsWatcherViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }

    private void ButtonClear_Click(object sender, RoutedEventArgs e)
    {
        ViewModel.KeysLogs.Clear();
    }

    private void KeyCommandsWatcher_OnClosed(object Sender, EventArgs E)
    {
        ViewModel.Dispose();
    }


    public class KeyCommandsWatcherViewModel : WindowViewModel, IDisposable
    {
        public KeyCommandsWatcherViewModel()
        {
            Title = "Лог команд клавиатуры";
            AppLog.KeyCommandReport += KeyCommandReport;
        }

        private void KeyCommandReport(string message)
        {
            KeysLogs.Add(message);
        }

        #region KeysLogs : ObservableCollection<string> - Лог команд

        /// <summary>Лог команд</summary>
        private ObservableCollection<string> _KeysLogs = new();

        /// <summary>Лог команд</summary>
        public ObservableCollection<string> KeysLogs
        {
            get => _KeysLogs;
            set => Set(ref _KeysLogs, value);
        }

        #endregion


        public void Dispose()
        {
            AppLog.KeyCommandReport -= KeyCommandReport;
            Debug.WriteLine("Вьюмодель логгера клавиатуры уничтожена");
        }
    }

}
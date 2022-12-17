using System.Collections.ObjectModel;
using JoyMapper.Models;
using JoyMapper.Services;
using WPR.MVVM.Commands;

namespace JoyMapper.ViewModels
{
    public partial class MainWindowViewModel
    {

        private readonly ProfileWorker _ProfileWorker = new();


        #region LogMessages : ObservableCollection<LogMessage> - Лог запущенного профиля

        /// <summary>Лог запущенного профиля</summary>
        private ObservableCollection<LogMessage> _LogMessages = new();

        /// <summary>Лог запущенного профиля</summary>
        public ObservableCollection<LogMessage> LogMessages
        {
            get => _LogMessages;
            set => Set(ref _LogMessages, value);
        }

        #endregion


        #region IsProfileStarted : bool - Запущен ли профиль

        /// <summary>Запущен ли профиль</summary>
        public bool IsProfileStarted => ActiveProfile != null;

        #endregion


        #region ActiveProfile : Profile - Запущенный профиль

        /// <summary>Запущенный профиль</summary>
        private Profile _ActiveProfile;

        /// <summary>Запущенный профиль</summary>
        public Profile ActiveProfile
        {
            get => _ActiveProfile;
            set => IfSet(ref _ActiveProfile, value).CallPropertyChanged(nameof(IsProfileStarted));
        }

        #endregion


        #region Command StartProfileCommand - Запустить профиль

        /// <summary>Запустить профиль</summary>
        private Command _StartProfileCommand;

        /// <summary>Запустить профиль</summary>
        public Command StartProfileCommand => _StartProfileCommand
            ??= new Command(OnStartProfileCommandExecuted, CanStartProfileCommandExecute, "Запустить профиль");

        /// <summary>Проверка возможности выполнения - Запустить профиль</summary>
        private bool CanStartProfileCommandExecute(object p) => p is Profile;

        /// <summary>Логика выполнения - Запустить профиль</summary>
        private void OnStartProfileCommandExecuted(object p)
        {
            LogMessages.Clear();
            var profile = (Profile)p;
            ActiveProfile = profile;
            _ProfileWorker.Start(profile);
        }

        #endregion


        #region Command StopProfileCommand - Остановить профиль

        /// <summary>Остановить профиль</summary>
        private Command _StopProfileCommand;

        /// <summary>Остановить профиль</summary>
        public Command StopProfileCommand => _StopProfileCommand
            ??= new Command(OnStopProfileCommandExecuted, CanStopProfileCommandExecute, "Остановить профиль");

        /// <summary>Проверка возможности выполнения - Остановить профиль</summary>
        private bool CanStopProfileCommandExecute() => IsProfileStarted;

        /// <summary>Логика выполнения - Остановить профиль</summary>
        private void OnStopProfileCommandExecuted()
        {
            ActiveProfile = null;
            _ProfileWorker.Stop();
        }

        #endregion

    }
}

using JoyMapper.Models;
using WPR.MVVM.Commands;

namespace JoyMapper.ViewModels
{
    internal partial class MainWindowViewModel
    {

        #region IsProfileStarted : bool - Запущен ли профиль

        /// <summary>Запущен ли профиль</summary>
        public bool IsProfileStarted
        {
            get => ActiveProfile != null;
        }

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
            var profile = (Profile)p;
            ActiveProfile = profile;
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
        }

        #endregion

    }
}

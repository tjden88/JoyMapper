using System.Collections.Generic;
using System.Collections.ObjectModel;
using JoyMapper.Interfaces;
using JoyMapper.Models.JoyBindings.Base;
using WPR.MVVM.Commands.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

internal class AudioPlayerViewModel : ViewModel
{
    #region Props

    #region AudioStreams : ObservableCollection<IAudioStream> - Потоки воспроизведения

    /// <summary>Потоки воспроизведения</summary>
    private ObservableCollection<IAudioStream> _AudioStreams;

    /// <summary>Потоки воспроизведения</summary>
    public ObservableCollection<IAudioStream> AudioStreams
    {
        get => _AudioStreams;
        set => Set(ref _AudioStreams, value);
    }

    #endregion


    public IEnumerable<ConfigButtonSetup> ButtonsConfigs { get; } = new List<ConfigButtonSetup>
    {
        new("Старт/Стоп воспроизведения", false),
        new("Следующая радиостанция", false),
        new("Предыдущая радиостанция", false),
        new("Ось регулировки громкости", true),
    };

    #endregion

    public class ConfigButtonSetup : ViewModel
    {
        public string Name { get; }
        private readonly bool _IsAxis;

        public ConfigButtonSetup(string Name, bool IsAxis)
        {
            this.Name = Name;
            _IsAxis = IsAxis;
        }

        public string ButtonCaption => _IsAxis ? "Назначить ось" : "Назначить кнопку";


        #region BindingBase : JoyBindingBase - Назанченная кнопка / ось

        /// <summary>Назанченная кнопка / ось</summary>
        private JoyBindingBase _BindingBase;

        /// <summary>Назанченная кнопка / ось</summary>
        public JoyBindingBase BindingBase
        {
            get => _BindingBase;
            set => Set(ref _BindingBase, value);
        }

        #endregion

        #region Command SetBindingCommand - Установить привязку

        /// <summary>Установить привязку</summary>
        private Command _SetBindingCommand;

        /// <summary>Установить привязку</summary>
        public Command SetBindingCommand => _SetBindingCommand
            ??= new Command(OnSetBindingCommandExecuted, CanSetBindingCommandExecute, "Установить привязку");

        /// <summary>Проверка возможности выполнения - Установить привязку</summary>
        private bool CanSetBindingCommandExecute() => true;

        /// <summary>Логика выполнения - Установить привязку</summary>
        private void OnSetBindingCommandExecuted()
        {
            
        }

        #endregion

        
    }

}
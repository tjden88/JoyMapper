using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels
{
    internal class AddPatternViewModel : WindowViewModel
    {


        #region Props


        #region IsPressRecorded : bool - Нажата ли кнопка записи нажатий

        /// <summary>Нажата ли кнопка записи нажатий</summary>
        private bool _IsPressRecorded;

        /// <summary>Нажата ли кнопка записи нажатий</summary>
        public bool IsPressRecorded
        {
            get => _IsPressRecorded;
            set => IfSet(ref _IsPressRecorded, value).CallPropertyChanged(nameof(PressButtonText));
        }

        #endregion


        #region IsReleaseRecorded : bool - Нажата ли кнопка записи при отпускании

        /// <summary>Нажата ли кнопка записи при отпускании</summary>
        private bool _IsReleaseRecorded;

        /// <summary>Нажата ли кнопка записи при отпускании</summary>
        public bool IsReleaseRecorded
        {
            get => _IsReleaseRecorded;
            set => IfSet(ref _IsReleaseRecorded, value).CallPropertyChanged(nameof(ReleaseButtonText));
        }

        #endregion


        #region PressButtonText : string - Текст кнопки записи нажатия

        /// <summary>Текст кнопки записи нажатия</summary>
        public string PressButtonText => IsPressRecorded ? "Остановть запись" : "Начать запись";

        #endregion


        #region ReleaseButtonText : string - Текст кнопки записи отпускания

        /// <summary>Текст кнопки записи отпускания</summary>
        public string ReleaseButtonText => IsReleaseRecorded ? "Остановть запись" : "Начать запись";

        #endregion

        

        #endregion



    }
}

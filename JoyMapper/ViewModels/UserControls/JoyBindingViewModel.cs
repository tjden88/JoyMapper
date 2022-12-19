using System;
using System.Diagnostics;
using JoyMapper.Interfaces;
using JoyMapper.Models.JoyBindings;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using JoyMapper.Services.Interfaces;
using WPR.MVVM.Commands;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class JoyBindingViewModel : ViewModel, IDisposable, IEditModel<JoyBindingBase>
{
    private readonly IJoyListener _JoyListener;
    private readonly AppWindowsService _AppWindowsService;

    public JoyBindingViewModel(): base(true)
    {
        JoyBinding = new AxisJoyBinding
        {
            JoyName ="Test",
            EndValue = 5000,
            StartValue = 200
        };
    }

    public JoyBindingViewModel(IJoyListener JoyListener, AppWindowsService AppWindowsService)
    {
        _JoyListener = JoyListener;
        _AppWindowsService = AppWindowsService;
    }


    #region JoyBinding : JoyBindingBase - Действие кнопки или оси

    /// <summary>Действие кнопки или оси</summary>
    private JoyBindingBase _JoyBinding;

    /// <summary>Действие кнопки или оси</summary>
    public JoyBindingBase JoyBinding
    {
        get => _JoyBinding;
        set => IfSet(ref _JoyBinding, value)
            .CallPropertyChanged(nameof(IsNormal))
            .CallPropertyChanged(nameof(IsReverse))
            .CallPropertyChanged(nameof(IsSwitch))
            .CallPropertyChanged(nameof(ActionTypeInfo))
            .CallPropertyChanged(nameof(AxisJoyBinding))
            .Then(WatchAction)
        ;
    }

    #endregion


    #region AxisJoyBinding : AxisJoyBinding - Настройки диапазона оси

    /// <summary>Настройки диапазона оси</summary>
    public AxisJoyBinding AxisJoyBinding => JoyBinding is AxisJoyBinding { } ax ? ax : null;

    #endregion


    #region ActionType - тип действия

    public bool IsNormal
    {
        get => JoyBinding?.ActivationType == JoyBindingBase.ActivationTypes.Normal;
        set
        {
            if (value && JoyBinding != null)
            {
                JoyBinding.ActivationType = JoyBindingBase.ActivationTypes.Normal;
                OnPropertyChanged(nameof(ActionTypeInfo));
            }
        }
    }

    public bool IsReverse
    {
        get => JoyBinding?.ActivationType == JoyBindingBase.ActivationTypes.Reverse;
        set
        {
            if (value && JoyBinding != null)
            {
                JoyBinding.ActivationType = JoyBindingBase.ActivationTypes.Reverse;
                OnPropertyChanged(nameof(ActionTypeInfo));
            }
        }
    }

    public bool IsSwitch
    {
        get => JoyBinding?.ActivationType == JoyBindingBase.ActivationTypes.Switch;
        set
        {
            if (value && JoyBinding != null)
            {
                JoyBinding.ActivationType = JoyBindingBase.ActivationTypes.Switch;
                OnPropertyChanged(nameof(ActionTypeInfo));
            }
        }
    }

    #endregion


    #region ActionTypeInfo

    public string ActionTypeInfo => JoyBinding?.ActivationType switch
    {
        JoyBindingBase.ActivationTypes.Normal =>
            "Действие становится активным, когда нажата кнопка или ось находится в назначенном диапазоне\n" +
            "Когда кнопка отпускается или ось выходит за назначенный диапазон, действие деактивируется",
        JoyBindingBase.ActivationTypes.Reverse =>
            "Действие становится активным, когда кнопка отпущена или ось находится вне назначенного диапазона\n" +
            "Когда кнопка нажимается или ось входит в назначенный диапазон, действие деактивируется\n" +
            "Этот режим является противоположным обычному, то есть действия при активации сработают, когда кнопка будет отпущена",
        JoyBindingBase.ActivationTypes.Switch =>
            "Действие будет активировано / деактивировано каждый раз, когда кнопка нажимается или отпускается, или когда ось входит в назначенный диапазон\n" +
            "При отпускании кнопки или выходе из назначенной зоны оси, статус действия не изменяется",
        null => "Назначьте кнопку или ось",
        _ => "Не выбрано"
    };

    #endregion

    #region WatchStatus


    private void WatchAction()
    {
        if (JoyBinding is null)
        {
            _JoyListener.StopListen();
            return;
        }

        _JoyListener.StartListen(new[] { JoyBinding });
        Debug.WriteLine($"Начато отслеживания кнопки {JoyBinding.Description}");
    }




    #endregion


    #region Commands

    #region Command AttachJoyButtonCommand - Определить кнопку джойстика

    /// <summary>Определить кнопку джойстика</summary>
    private Command _AttachJoyButtonCommand;

    /// <summary>Определить кнопку джойстика</summary>
    public Command AttachJoyButtonCommand => _AttachJoyButtonCommand
        ??= new Command(OnAttachJoyButtonCommandExecuted, CanAttachJoyButtonCommandExecute, "Определить кнопку джойстика");

    /// <summary>Проверка возможности выполнения - Определить кнопку джойстика</summary>
    private bool CanAttachJoyButtonCommandExecute() => true;

    /// <summary>Логика выполнения - Определить кнопку джойстика</summary>
    private void OnAttachJoyButtonCommandExecuted()
    {
        var bind = _AppWindowsService.GetJoyBinding();
        if (bind is null) return;
        JoyBinding = bind;
    }

    #endregion



    #endregion

    public void Dispose()
    {
        _JoyListener.StopListen();
        Debug.WriteLine($"Отслеживание кнопки {JoyBinding.Description} остановлено");
    }

    public JoyBindingBase GetModel() => JoyBinding;

    public void SetModel(JoyBindingBase model) => JoyBinding = model;
}
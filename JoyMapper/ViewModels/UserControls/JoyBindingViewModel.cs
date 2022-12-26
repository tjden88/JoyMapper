using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using JoyMapper.Interfaces;
using JoyMapper.Models.JoyBindings;
using JoyMapper.Models.JoyBindings.Base;
using JoyMapper.Services;
using JoyMapper.Services.Interfaces;
using WPR.MVVM.Commands.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.UserControls;

public class JoyBindingViewModel : ViewModel, IDisposable, IEditModel<JoyBindingBase>
{
    private readonly IJoyBindingListener _JoyBindingListener;
    private readonly AppWindowsService _AppWindowsService;

    /// <summary> Сообщает об изменении состояния привязки </summary>
    public Action<bool> BindingStateChanged { get; set; }

    public JoyBindingViewModel(): base(true)
    {
        JoyBinding = new AxisJoyBinding
        {
            JoyName ="Test",
            EndValue = 5000,
            StartValue = 200
        };
    }

    public JoyBindingViewModel(IJoyBindingListener JoyBindingListener, AppWindowsService AppWindowsService)
    {
        _JoyBindingListener = JoyBindingListener;
        _AppWindowsService = AppWindowsService;
        _JoyBindingListener.ChangesHandled += Listener_OnChangesHandled;
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
            _JoyBindingListener.StopListen();
            return;
        }
        _JoyBindingListener.StartListen(new[] { JoyBinding });
        Debug.WriteLine($"Начато отслеживания кнопки {JoyBinding.Description}");
    }


    private void Listener_OnChangesHandled(IEnumerable<JoyBindingBase> bindings)
    {
        BindingStateChanged?.Invoke(bindings.First().IsActive);
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
        _JoyBindingListener.StopListen();
        _JoyBindingListener.ChangesHandled -= Listener_OnChangesHandled;
        Debug.WriteLine($"Отслеживание кнопки {JoyBinding?.Description} остановлено");
    }

    public JoyBindingBase GetModel() => JoyBinding;

    public void SetModel(JoyBindingBase model) => JoyBinding = model;
}
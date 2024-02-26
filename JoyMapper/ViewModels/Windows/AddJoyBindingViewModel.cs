using JoyMapper.Services.Interfaces;
using System.Collections.Generic;
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using JoyMapper.Models;
using JoyMapper.Models.JoyBindings;
using JoyMapper.Models.JoyBindings.Base;
using WPR.MVVM.ViewModels;

namespace JoyMapper.ViewModels.Windows;

public class AddJoyBindingViewModel : ViewModel
{
    public enum BindingFilters
    {
        All,
        Buttons,
        Axes
    }

    private readonly IJoystickStateManager _JoystickStateManager;
    private readonly IJoyBindingListener _JoyBindingListener;


    public AddJoyBindingViewModel(IJoystickStateManager JoystickStateManager, IJoyBindingListener JoyBindingListener)
    {
        _JoystickStateManager = JoystickStateManager;
        _JoyBindingListener = JoyBindingListener;
    }

    #region Prop

    /// <summary>Заголовок</summary>
    public string Title => Filter switch
    {
        BindingFilters.All => "Назначить кнопку/ось",
        BindingFilters.Buttons => "Назначить кнопку",
        BindingFilters.Axes => "Назначить ось",
        _ => throw new ArgumentOutOfRangeException(nameof(Filter), Filter, null)
    };


    #region Filter : BindingFilters - Фильтр назначаемых действий

    /// <summary>Фильтр назначаемых действий</summary>
    private BindingFilters _Filter;

    /// <summary>Фильтр назначаемых действий</summary>
    public BindingFilters Filter
    {
        get => _Filter;
        set => IfSet(ref _Filter, value)
            .CallPropertyChanged(nameof(Title))
            .CallPropertyChanged(nameof(JoyName))
        ;
    }

    #endregion

    private string Hint => Filter switch
    {
        BindingFilters.Axes => "Сдвиньте ось джойстика",
        BindingFilters.Buttons => "Нажмите кнопку",
        BindingFilters.All => "Нажмите кнопку или сдвиньте ось джойстика",
        _ => throw new ArgumentOutOfRangeException(nameof(Filter), Filter, null)
    };

    #region JoyBinding : JoyBindingBase - Привязка кнопки

    /// <summary>Привязка кнопки</summary>
    private JoyBindingBase _JoyBinding;

    /// <summary>Привязка кнопки</summary>
    public JoyBindingBase JoyBinding
    {
        get => _JoyBinding;
        set => IfSet(ref _JoyBinding, value)
            .CallPropertyChanged(nameof(JoyName))
            .CallPropertyChanged(nameof(Description))
            .CallPropertyChanged(nameof(IsBindingSelected));
    }

    #endregion

    public string JoyName => JoyBinding?.JoyName ?? Hint;

    public string Description => JoyBinding?.Description;

    public bool IsBindingSelected => JoyBinding != null;


    #endregion


    /// <summary> Начать отслеживание кнопок </summary>
    public void StartWatching()
    {
        var allBindings = new List<JoyBindingBase>();
        var connectedJoys = _JoystickStateManager.GetConnectedJoysticks();

        foreach (var connectedJoy in connectedJoys) 
            allBindings.AddRange(AllJoyBindings(connectedJoy));

        _JoyBindingListener.ChangesHandled += Listener_OnChangesHandled;
        _JoyBindingListener.StartListen(allBindings);
        Debug.WriteLine("Начато отслеживание всех кнопок всех джойстиков");
    }

    private void Listener_OnChangesHandled(JoyBindingBase changes)
    {
        if (changes is not {IsActive: true} change) return;

        Debug.WriteLine(change.Description);
        JoyBinding = change;
        Application.Current.Dispatcher.Invoke(CommandManager.InvalidateRequerySuggested);
    }

    /// <summary> Остановить отслеживание </summary>
    public void StopWatching()
    {
        _JoyBindingListener.StopListen();
        _JoyBindingListener.ChangesHandled -= Listener_OnChangesHandled;
        Debug.WriteLine("Завершено отслеживание всех кнопок всех джойстиков");
    }


    #region AllBindings

    private List<JoyBindingBase> AllJoyBindings(string JoyName)
    {
        var list = new List<JoyBindingBase>();
        if (Filter != BindingFilters.Axes)
        {
            // Кнопки
            for (var i = 1; i <= 128; i++)
            {
                list.Add(new ButtonJoyBinding
                {
                    JoyName = JoyName,
                    ButtonNumber = i
                });
            }

            var powValues = new[] { 0, 4500, 9000, 13500, 18000, 22500, 27000, 31500 };

            // POW
            foreach (var powValue in powValues)
            {
                list.Add(new PowJoyBinding
                {
                    JoyName = JoyName,
                    PowNumber = PowJoyBinding.PowNumbers.Pow1,
                    PowValue = powValue
                });

                list.Add(new PowJoyBinding
                {
                    JoyName = JoyName,
                    PowNumber = PowJoyBinding.PowNumbers.Pow2,
                    PowValue = powValue
                });
            }
        }


        if (Filter == BindingFilters.Buttons) return list;

        // Оси
        foreach (JoyAxises axis in Enum.GetValues(typeof(JoyAxises)))
        {
            list.Add(new AxisJoyBinding
            {
                Axis = axis,
                StartValue = 0,
                EndValue = 20000,
                JoyName = JoyName
            });
            list.Add(new AxisJoyBinding
            {
                Axis = axis,
                StartValue = 45000,
                EndValue = 65535,
                JoyName = JoyName
            });
        }

        return list;
    }

    #endregion

}
using System;
using System.Diagnostics;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using JoyMapper.Interfaces;
using JoyMapper.Models;
using JoyMapper.ViewModels.Windows;
using WPR.Dialogs;

namespace JoyMapper.Views.Windows;

/// <summary>
/// Логика взаимодействия для EditPattern.xaml
/// </summary>
public partial class EditPattern : IEditModel<JoyPattern>
{
    public EditPatternViewModel ViewModel { get; }

    public EditPattern(EditPatternViewModel ViewModel)
    {
        this.ViewModel = ViewModel;
        InitializeComponent();
    }


    private void EditPatternWindow_OnLoaded(object Sender, RoutedEventArgs E)
    {
        if(ViewModel.JoyBindingViewModel.JoyBinding == null)
            ViewModel.JoyBindingViewModel.AttachJoyButtonCommand.Execute();
    }

    private void ScrollViewer_OnScrollChanged(object Sender, ScrollChangedEventArgs E)
    {
        if (E.ExtentHeightChange > 0) ScrollViewer.ScrollToEnd();
    }

    private async void ButtonSave_OnClick(object Sender, RoutedEventArgs E)
    {
        var vm = ViewModel;

        if (vm.JoyBindingViewModel.JoyBinding == null)
        {
            await WPRDialogHelper.InformationAsync(this, "Не определена кнопка или ось контроллера для назначения паттерна");
            return;
        }

        if (vm.PatternActionViewModel.SelectedPatternAction == null)
        {
            await WPRDialogHelper.InformationAsync(this, "Не выбрано действие паттерна");
            return;
        }

        if (!vm.PatternActionViewModel.SelectedPatternAction.IsValid(out var errorMessage))
        {
            await WPRDialogHelper.InformationAsync(this, errorMessage);
            return;
        }

        var patternName = vm.PatternName?.Trim();

        if (string.IsNullOrEmpty(patternName))
        {
            await WPRDialogHelper.InformationAsync(this, "Введите имя паттерна");
            return;
        }

        vm.PatternName = patternName;
        DialogResult = true;
    }

    public JoyPattern GetModel()
    {
        var pattern = new JoyPattern
        {
            Id = ViewModel.Id,
            Name = ViewModel.PatternName,
            Binding = ViewModel.JoyBindingViewModel.GetModel(),
            GroupName = string.IsNullOrWhiteSpace(ViewModel.GroupName) ? null : ViewModel.GroupName.Trim(),
            PatternAction = ViewModel.PatternActionViewModel.GetModel(),
            ModificatorId = ViewModel.SelectedModificator?.Id ?? 0,
        };
        return pattern;
    }

    public void SetModel(JoyPattern model)
    {
        ViewModel.Id = model.Id;
        ViewModel.GroupName = model.GroupName;
        ViewModel.PatternName = model.Name;
        ViewModel.JoyBindingViewModel.SetModel(model.Binding);
        ViewModel.PatternActionViewModel.SetModel(model.PatternAction);
        ViewModel.SelectedModificator = ViewModel.Modificators
                .FirstOrDefault(m => m.Id == model.ModificatorId);

        Title = $"Редактирование паттерна {model.Name}";
    }

    private void EditPattern_OnClosed(object Sender, EventArgs E)
    {
        ViewModel.Dispose();
        Debug.WriteLine("ViewModel паттерна уничтожена");
    }
}
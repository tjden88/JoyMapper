using System.Windows;
using JoyMapper.Interfaces;
using JoyMapper.Models;
using JoyMapper.ViewModels.Windows;
using WPR.Dialogs;

namespace JoyMapper.Views.Windows;

/// <summary>
/// Логика взаимодействия для EditModificator.xaml
/// </summary>
public partial class EditModificator : Window, IEditModel<Modificator>
{
    public EditModificatorViewModel ViewModel { get; }

    public EditModificator(EditModificatorViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = viewModel;
        InitializeComponent();
    }

    public Modificator GetModel() =>
        new()
        {
            Id = ViewModel.Id,
            Name = ViewModel.Name,
            Binding = ViewModel.JoyBindingViewModel.GetModel()
        };

    public void SetModel(Modificator model)
    {
        ViewModel.Id = model.Id;
        ViewModel.Name = model.Name;
        ViewModel.JoyBindingViewModel.SetModel(model.Binding);
        ViewModel.Title = $"Редактирование модификатора {model.Name}";
    }

    private async void ButtonSave_Click(object sender, RoutedEventArgs e)
    {
        if (ViewModel.JoyBindingViewModel.JoyBinding == null)
        {
            await WPRDialogHelper.InformationAsync(App.ActiveWindow, "Не определена кнопка или ось модификатора");
            return;
        }


        var name = ViewModel.Name?.Trim();

        if (string.IsNullOrEmpty(name))
        {
            await WPRDialogHelper.InformationAsync(App.ActiveWindow, "Введите имя модификатора");
            return;
        }

        ViewModel.Name = name;
        DialogResult = true;
    }

    private void EditModificator_OnLoaded(object Sender, RoutedEventArgs E)
    {
        if(ViewModel.JoyBindingViewModel.JoyBinding is null)
            ViewModel.JoyBindingViewModel.AttachJoyButtonCommand.Execute();
    }
}
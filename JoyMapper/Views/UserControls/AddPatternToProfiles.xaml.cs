using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using JoyMapper.Interfaces;
using JoyMapper.Models;
using JoyMapper.Services.Data;
using WPR.Domain.Interfaces;
using WPR.MVVM.ViewModels;

namespace JoyMapper.Views.UserControls;

/// <summary>
/// Логика взаимодействия для AddPatternToProfiles.xaml
/// </summary>
public partial class AddPatternToProfiles : IWPRDialog, IEditModel<JoyPattern>
{

    public ObservableCollection<SelectedProfile> SelectedProfiles { get; set; }

    public JoyPattern Pattern { get; set; }

    public AddPatternToProfiles(DataManager DataManager)
    {
        var allProfiles = DataManager.Profiles;
        SelectedProfiles = new(allProfiles.Select(p => new SelectedProfile {Profile = p}));

        InitializeComponent();
    }

    public object DialogContent => this;
    public bool StaysOpen => true;

    public event Action<bool> Completed;

    public JoyPattern GetModel()
    {
        return Pattern;
    }

    public void SetModel(JoyPattern model)
    {
        Pattern = model;
        foreach (var selectedProfile in SelectedProfiles)
            selectedProfile.IsSelected = selectedProfile.Profile.PatternsIds.Contains(model.Id);
    }

    private void ButtonOk_OnClick(object Sender, RoutedEventArgs E) => Completed?.Invoke(true);

    private void ButtonCancel_OnClick(object Sender, RoutedEventArgs E) => Completed?.Invoke(false);


    public class SelectedProfile : ViewModel
    {
        #region IsSelected : bool - Выбрано

        /// <summary>Выбрано</summary>
        private bool _IsSelected;

        /// <summary>Выбрано</summary>
        public bool IsSelected
        {
            get => _IsSelected;
            set => Set(ref _IsSelected, value);
        }

        #endregion

        #region Profile : Profile - Профиль

        /// <summary>Профиль</summary>
        private Profile _Profile;

        /// <summary>Профиль</summary>
        public Profile Profile
        {
            get => _Profile;
            set => Set(ref _Profile, value);
        }

        #endregion

    }

}
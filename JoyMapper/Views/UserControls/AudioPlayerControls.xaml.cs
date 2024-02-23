using JoyMapper.ViewModels.UserControls;

namespace JoyMapper.Views.UserControls;


public partial class AudioPlayerControls
{
    public AudioPlayerControlsViewModel ViewModel { get; }

    public AudioPlayerControls(AudioPlayerControlsViewModel viewModel)
    {
        ViewModel = viewModel;
        InitializeComponent();
    }


   
}
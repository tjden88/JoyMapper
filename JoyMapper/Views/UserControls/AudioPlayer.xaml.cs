using System;
using System.IO;
using System.Net;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using NAudio.Wave;
using SharpDX.Multimedia;

namespace JoyMapper.Views.UserControls
{
    /// <summary>
    /// Логика взаимодействия для AudioPlayer.xaml
    /// </summary>
    public partial class AudioPlayer : UserControl
    {
        public AudioPlayer()
        {
            InitializeComponent();
        }

        private void ButtonBase_OnClick(object sender, RoutedEventArgs e)
        {
            PlayMp3FromUrl("https://stream.deep1.ru/deep1aac");
        }

        public static void PlayMp3FromUrl(string url)
        {
            using var mf = new MediaFoundationReader(url);
            using var wo = new WaveOutEvent();
            wo.Init(mf);
            wo.Play();
        }
    }
}

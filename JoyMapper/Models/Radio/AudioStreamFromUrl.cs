using System;
using System.Net.Http;
using System.Threading.Tasks;
using JoyMapper.Interfaces;
using NAudio.Wave;
using WPR.Icons;

namespace JoyMapper.Models.Radio;

/// <summary>
/// Потоковое интернет-радио
/// </summary>
internal class AudioStreamFromUrl : IAudioStream
{
    private readonly string _Url;
    private WaveOutEvent _Stream;

    public event EventHandler<string> PlaybackError;


    public AudioStreamFromUrl(string StreamUrl)
    {
        _Url = StreamUrl;
    }


    public async Task<bool> IsAvaliable()
    {
        try
        {
            using var http = new HttpClient();
            var response = await http.SendAsync(new HttpRequestMessage(HttpMethod.Head, _Url));
            return response.IsSuccessStatusCode;
        }
        catch(Exception )
        {
            return false;
        }
    }

    public bool IsPlaying => _Stream is { PlaybackState: PlaybackState.Playing };

    public void Play()
    {
        if (IsPlaying) return;
        Dispose();


        try
        {
            using var mf = new MediaFoundationReader(_Url);
            var wo = new WaveOutEvent();
            wo.Init(mf);
            
            _Stream = wo;
            _Stream.PlaybackStopped += WoOnPlaybackStopped;
            _Stream.Play();
        }
        catch(Exception )
        {
            PlaybackError?.Invoke(this,  $"Невозможно воспроизвести: {_Url}");
        }

    }

    private void WoOnPlaybackStopped(object sender, StoppedEventArgs e) => PlaybackError?.Invoke(this, $"Ошибка воспроизведения: {_Url}");

    public void Stop() => Dispose();

    public void SetVolume(byte volume)
    {
        if(_Stream is null) return;
        _Stream.Volume = (float)(volume / 255.0);
    }

    public string Source => _Url;

    public PackIconKind Icon => PackIconKind.Internet;

    public void Dispose()
    {
        if (_Stream is not null)
        {
            _Stream.PlaybackStopped -= WoOnPlaybackStopped;
            _Stream.Stop();
        }

        _Stream?.Dispose();
        _Stream = null;
    }

    public bool Equals(IAudioStream other) => other is AudioStreamFromUrl stream && stream._Url.Equals(_Url);
}
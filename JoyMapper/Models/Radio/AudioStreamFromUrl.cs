using System;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using JoyMapper.Interfaces;
using NAudio.Wave;

namespace JoyMapper.Models.Radio;

/// <summary>
/// Потоковое интернет-радио
/// </summary>
internal class AudioStreamFromUrl : IAudioStream
{
    private readonly string _Url;
    private WaveStream _Stream;

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
            http.Timeout = TimeSpan.FromSeconds(5);
            var response = await http.SendAsync(new HttpRequestMessage(HttpMethod.Head, _Url));
            return response.IsSuccessStatusCode;
        }
        catch(Exception )
        {
            return false;
        }
    }

    public bool IsPlaying => _Stream is { SoundOut.PlaybackState: PlaybackState.Playing };

    public void Play()
    {
        if (IsPlaying) return;
        Dispose();


        try
        {
            using var mf = new MediaFoundationReader(_Url);
            var channel = new WaveChannel32(new MediaFoundationReader(_Url));
            //var wo = new DirectSoundOut(Guid.Parse("139c7588-8346-4530-b2d4-544429567429"));
            var wo = new DirectSoundOut();
            wo.Init(channel);
            
            _Stream = new WaveStream(wo, channel);
            wo.PlaybackStopped += WoOnPlaybackStopped;
            wo.Play();
        }
        catch(Exception e)
        {
            PlaybackError?.Invoke(this,  $"Невозможно воспроизвести: {_Url}");
        }

    }

    private void WoOnPlaybackStopped(object sender, StoppedEventArgs e) => PlaybackError?.Invoke(this, $"Ошибка воспроизведения: {_Url}");

    public void Stop() => Dispose();

    public void SetVolume(byte volume)
    {
        if(_Stream is null) return;
        var value = (float)volume / 127;
        Debug.WriteLine(value);
        _Stream.Channel.Volume = value;
    }

    public string Source => _Url;


    public void Dispose()
    {
        if (_Stream is not null)
        {
            _Stream.SoundOut.PlaybackStopped -= WoOnPlaybackStopped;
            _Stream.SoundOut.Stop();
        }

        _Stream?.Dispose();
        _Stream = null;
    }

    public bool Equals(IAudioStream other) => other is AudioStreamFromUrl stream && stream._Url.Equals(_Url);


    private record WaveStream(DirectSoundOut SoundOut, WaveChannel32 Channel): IDisposable
    {
        public void Dispose()
        {
            SoundOut?.Dispose();
            Channel?.Dispose();
        }
    }
}
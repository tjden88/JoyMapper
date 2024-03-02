using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using JoyMapper.Interfaces;
using NAudio.Wave;

namespace JoyMapper.Models.Radio;

/// <summary>
/// Радио из локальной папки
/// </summary>
[Obsolete("Не доделано")]
internal class AudioStreamFromDirectory : IAudioStream
{
    public AudioStreamFromDirectory(string Path)
    {
        Source = Path;
    }

    public bool IsPlaying => throw new NotImplementedException();

    public string Source { get; }

    public event EventHandler<string> PlaybackError;

    public void Dispose()
    {
        throw new NotImplementedException();
    }

    public bool Equals(IAudioStream other) => 
        other is { } oth && oth.Source == Source;

    public Task<bool> IsAvaliable()
    {
        return Task.FromResult(Directory.Exists(Source));
    }

    public void Play(Guid? OutputDeviceId)
    {
        using (var audioFile = new AudioFileReader(null))
        using (var outputDevice = new WaveOutEvent())
        {
            outputDevice.Init(audioFile);
            outputDevice.Play();
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                Thread.Sleep(1000);
            }
        }
    }

    public void SetVolume(byte volume)
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }
}
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using JoyMapper.Interfaces;
using JoyMapper.Models;
using JoyMapper.Models.Radio;
using JoyMapper.Services.Data;

namespace JoyMapper.Services;

public class AudioPlayerService
{
    private readonly DataManager _DataManager;
    public event EventHandler<bool> IsPlayingChanged;

    public event EventHandler<string> SourceChanged;

    public AudioPlayerService(DataManager dataManager)
    {
        _DataManager = dataManager;
    }


    private bool _IsPlaying;

    /// <summary>Статус воспроизведения</summary>
    public bool IsPlaying
    {
        get => _IsPlaying;
        private set
        {
            if (Equals(value, _IsPlaying))
                return;
            _IsPlaying = value;
            IsPlayingChanged?.Invoke(this, value);
        }
    }

    #region CurrentSource : string - Текущий источник

    /// <summary>Текущий источник</summary>
    private string _CurrentSource;

    /// <summary>Текущий источник</summary>
    public string CurrentSource
    {
        get => _CurrentSource;
        set
        {
            if (Equals(value, _CurrentSource)) return;
            _CurrentSource = value;
            SourceChanged?.Invoke(this, value);
        }
    }

    #endregion

    #region Volume : byte - Громкость

    /// <summary>Громкость</summary>
    private byte _Volume = 64;

    /// <summary>Громкость</summary>
    public byte Volume
    {
        get => _Volume;
        set
        {
            if (Equals(value, _Volume)) return;
            _Volume = value;
            Debug.WriteLine(value);
            _CurrentStream?.SetVolume(value);
        }
    }

    #endregion

    public bool CanPlay => _AudioStreams.Any();

    public bool CanNextPrevious => _AudioStreams.Count > 1;


    private readonly List<IAudioStream> _AudioStreams = new();

    private IAudioStream _CurrentStream;


    /// <summary>
    /// Загрузить источники
    /// </summary>
    public int LoadSources(IEnumerable<string> sources)
    {
        if (_AudioStreams.Any())
        {
            foreach (var audioStream in _AudioStreams)
            {
                audioStream.PlaybackError -= AudioStreamOnPlaybackError;
                audioStream.Dispose();
            }
            _AudioStreams.Clear();
            _CurrentStream = null;
        }

        var streams = sources
            .Select(CreateStream)
            .Where(s => s != null);

        foreach (var stream in streams)
        {
            stream.PlaybackError += AudioStreamOnPlaybackError;
            _AudioStreams.Add(stream);
        }

        return _AudioStreams.Count;
    }

    private void AudioStreamOnPlaybackError(object sender, string e)
    {
        AppLog.LogMessage($"Ошибка воспроизведения: {e}", LogMessage.MessageType.Warning);
        Next();
    }

    public void Play()
    {
        if (IsPlaying) return;
        if (!_AudioStreams.Any()) return;

        var stream = _CurrentStream;
        if (stream is null)
        {
            var next = Random.Shared.Next(_AudioStreams.Count);
            stream = _AudioStreams[next];
        }
        Play(stream);
    }

    public void Stop()
    {
        if (!IsPlaying) return;
        _CurrentStream?.Stop();
        CurrentSource = null;
        IsPlaying = false;
    }

    public void Next()
    {
        var currentStream = _CurrentStream;

        if (currentStream is null)
        {
            Play();
            return;
        }
        if (_AudioStreams.Count == 1)
            return;

        var index = _AudioStreams.IndexOf(currentStream);
        if (index == _AudioStreams.Count - 1)
            index = 0;
        else
            index++;

        currentStream.Stop();
        Play(_AudioStreams[index]);
    }

    public void Previous()
    {
        var currentStream = _CurrentStream;

        if (currentStream is null)
        {
            Play();
            return;
        }
        if (_AudioStreams.Count == 1)
            return;

        var index = _AudioStreams.IndexOf(currentStream);
        if (index == 0)
            index = _AudioStreams.Count - 1;
        else
            index--;

        currentStream.Stop();
        Play(_AudioStreams[index]);
    }

    /// <summary> Проверить доступность источника </summary>
    public async Task<bool> CheckAvaliable(string source)
    {
        var audioStream = CreateStream(source);
        return audioStream is not null && await audioStream.IsAvaliable();
    }

    private IAudioStream CreateStream(string source) => new AudioStreamFromUrl(source);

    private bool _IsLoading;
    private async void Play(IAudioStream audioStream)
    {
        if (_IsLoading)
            return;

        _IsLoading = true;
        SourceChanged?.Invoke(this, "Загружается...");
        await Task.Run(() =>
        {
            _CurrentStream = audioStream;
            audioStream.Play(_DataManager.RadioSettings.OutputDeviceId);
            audioStream.SetVolume(Volume);
            CurrentSource = audioStream.Name;
        });
        _IsLoading = false;
        IsPlaying = true;
    }
}
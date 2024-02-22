using System;
using System.Threading.Tasks;
using WPR.Icons;

namespace JoyMapper.Interfaces;

/// <summary>
/// Поток воспроизведения музыки
/// </summary>
public interface IAudioStream : IDisposable, IEquatable<IAudioStream>
{
    /// <summary>
    /// Происходит при ошибке воспроизведения
    /// </summary>
    event EventHandler<string> PlaybackError;

    /// <summary>
    /// Доступен ли источник воспроизведения
    /// </summary>
    Task<bool> IsAvaliable();

    /// <summary>
    /// Воспроизводится в текущий момент
    /// </summary>
    bool IsPlaying { get; }

    void Play();

    void Stop();

    /// <summary>
    /// Установить громкость 0-255
    /// </summary>
    void SetVolume(byte volume);

    /// <summary>
    /// Адрес источника
    /// </summary>
    string Source { get; }

    PackIconKind Icon { get; }
}

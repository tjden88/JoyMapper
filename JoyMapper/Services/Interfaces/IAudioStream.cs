using System;
using System.Threading.Tasks;

namespace JoyMapper.Services.Interfaces;

/// <summary>
/// Поток воспроизведения музыки
/// </summary>
internal interface IAudioStream
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
}

using System;
using JoyMapper.Models;

namespace JoyMapper.Services.Interfaces;

/// <summary> Прослушивание профиля </summary>
public interface IProfileListener
{
    event EventHandler<Profile> ProfileChanged;

    Profile CurrentProfile { get; }

    void StartListenProfile(Profile Profile);

    void StopListenProfile();
}
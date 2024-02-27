using System;
using JoyMapper.Models;

namespace JoyMapper.Services.Interfaces;

/// <summary> Прослушивание профиля </summary>
public interface IProfileListener
{
    event EventHandler<Profile> ProfileChanged;

    void StartListenProfile(Profile Profile);

    void StopListenProfile();
}
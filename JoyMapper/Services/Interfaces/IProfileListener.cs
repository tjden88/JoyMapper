using JoyMapper.Models;

namespace JoyMapper.Services.Interfaces;

/// <summary> Прослушивание профиля </summary>
public interface IProfileListener
{
    void StartListenProfile(Profile Profile);

    void StopListenProfile();
}
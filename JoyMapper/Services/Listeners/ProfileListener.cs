using System.Linq;
using JoyMapper.Models;
using JoyMapper.Services.Data;
using JoyMapper.Services.Interfaces;

namespace JoyMapper.Services.Listeners;

public class ProfileListener: IProfileListener
{
    private readonly IJoyPatternListener _JoyPatternListener;
    private readonly DataManager _DataManager;

    public ProfileListener(IJoyPatternListener JoyPatternListener, DataManager DataManager)
    {
        _JoyPatternListener = JoyPatternListener;
        _DataManager = DataManager;
    }

    public void StartListenProfile(Profile Profile)
    {
        var patterns = _DataManager.JoyPatterns
            .Where(p =>Profile.PatternGroups.Contains(p.GroupName) 
                       || Profile.PatternsIds.Contains(p.Id))
            .ToList();

        if (patterns.Count == 0)
        {
            AppLog.LogMessage("В выбранном профиле не назначено ни одного паттерна! Необходимо отредактировать профиль или запустить другой"
                , LogMessage.MessageType.Error);
            return;
        }
        AppLog.LogMessage("Профиль запущен");
        _JoyPatternListener.StartWatching(patterns);
    }

    public void StopListenProfile() => _JoyPatternListener.StopWatching();
}
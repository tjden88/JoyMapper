using System;
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
            .Where(p => Profile.PatternsIds
                .Contains(p.Id));

        _JoyPatternListener.StartWatching(patterns.ToList());
    }

    public void StopListenProfile() => _JoyPatternListener.StopWatching();
}
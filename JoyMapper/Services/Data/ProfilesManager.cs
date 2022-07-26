﻿using JoyMapper.Models;
using JoyMapper.Views.Windows;

namespace JoyMapper.Services.Data;

/// <summary>
/// Создание, изменение, копирование профилей
/// </summary>
public class ProfilesManager
{
    private readonly DataSerializer _DataSerializer;
    private readonly AppWindowsService _AppWindowsService;

    public ProfilesManager(DataSerializer DataSerializer, AppWindowsService AppWindowsService)
    {
        _DataSerializer = DataSerializer;
        _AppWindowsService = AppWindowsService;
    }

    public Profile AddProfile()
    {
        var wnd = _AppWindowsService.GetDialogWindow<EditProfile>();
        return wnd.ShowDialog() != true ? null : wnd.GetModel();
    }

    public Profile CopyProfile(Profile profile)
    {
        var newProf = _DataSerializer.CopyObject(profile);
        return newProf;
    }

    public Profile UpdateProfile(Profile profile)
    {
        var wnd = _AppWindowsService.GetDialogWindow<EditProfile>();
        wnd.SetModel(profile);
        return wnd.ShowDialog() != true ? null : wnd.GetModel();
    }

}
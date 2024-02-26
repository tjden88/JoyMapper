using System;
using System.Collections.Generic;
using JoyMapper.Models.Base;

namespace JoyMapper.Models;

/// <summary>
/// Настройки радио
/// </summary>
public class RadioSettings
{
    public bool IsEnabled { get; set; }

    public Guid? OutputDeviceId { get; set; }

    public JoyBindingBase PlayStopBinding { get; set; }
    public JoyBindingBase NextBinding { get; set; }
    public JoyBindingBase PreviousBinding { get; set; }
    public JoyBindingBase VolumeBinding { get; set; }

    public List<string> Sources { get; set; } = new();
}
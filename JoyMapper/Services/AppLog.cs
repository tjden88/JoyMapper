using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Threading;
using JoyMapper.Models;

namespace JoyMapper.Services;

/// <summary>
/// Отображение ошибок и информации приложения
/// </summary>
internal static class AppLog
{

    public static Action<LogMessage> Report;

    public static Action<string> KeyCommandReport;


    /// <summary> Лог работы профиля </summary>
    public static void LogMessage(string Message, LogMessage.MessageType messageType = Models.LogMessage.MessageType.Info)
    {
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
            new Action(() => Report?.Invoke(new LogMessage(messageType, Message))));

        Debug.WriteLine($"AppLog: {messageType} - {Message}");
    }


    /// <summary> Лог клавиатурных команд </summary>
    public static void LogKeyCommands(string Message)
    {
        Application.Current.Dispatcher.Invoke(DispatcherPriority.Background,
            new Action(() => KeyCommandReport?.Invoke(Message)));
    }
}
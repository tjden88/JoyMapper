using System;
using JoyMapper.Models;

namespace JoyMapper.Services
{
    /// <summary>
    /// Отображение ошибок и информации приложения
    /// </summary>
    internal static class AppLog
    {

        public static Action<LogMessage> Report;

        public static void LogMessage( string Message, LogMessage.MessageType messageType = Models.LogMessage.MessageType.Info) => 
            Report?.Invoke(new LogMessage(messageType, Message));
    }
}

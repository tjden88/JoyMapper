using System;

namespace JoyMapper.Services
{
    /// <summary>
    /// Отображение ошибок и информации приложения
    /// </summary>
    internal static class AppLog
    {
        public static Action<string> Report;

        public static void LogMessage(string Message)
        {
            var msg = DateTime.Now.ToString("T") + " - " + Message;
            Report?.Invoke(msg);
        }
    }
}

﻿using System.Linq;
using System.Windows;
using JoyMapper.Services;

namespace JoyMapper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        /// <summary> Версия приложения </summary>
        internal const string AppVersion = "1.3";

        /// <summary> Менеджер данных профилей текущей сессии </summary>
        internal static DataManager DataManager { get; } = new();

        /// <summary> Активное окно </summary>
        internal static Window ActiveWindow => Current.Windows.Cast<Window>().First(w => w.IsActive);

        /// <summary> Сервис проверки обновлений </summary>
        internal static UpdateChecker UpdateChecker { get; } = new();


    }

}

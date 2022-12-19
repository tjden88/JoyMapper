using JoyMapper.Views;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JoyMapper.Services;

/// <summary>
/// Сервис проверки обновлений
/// </summary>
public class UpdateChecker
{
    private readonly AppWindowsService _AppWindowsService;

    //private const string LatestVersionTxtUrl = @"https://raw.githubusercontent.com/tjden88/JoyMapper/v1.4/JoyMapper/LatestVersion.txt"; // Debug
    private const string LatestVersionTxtUrl = @"https://raw.githubusercontent.com/tjden88/JoyMapper/master/JoyMapper/LatestVersion.txt";


    private static string CurrentVersion => App.AppVersion;

    private LastVersion _LastVersion;

    public UpdateChecker(AppWindowsService AppWindowsService)
    {
        _AppWindowsService = AppWindowsService;
    }

    /// <summary>
    /// Проверить, есть ли новая версия
    /// </summary>
    /// <returns></returns>
    public async Task<bool> CheckUpdate()
    {
        var lv = await GetLastVersion();
        if (lv == null) return false;

        var version = new Version(lv.Version);
        var currVersion = new Version(CurrentVersion);

        var result = version.CompareTo(currVersion);

        return result > 0;
    }


    /// <summary>
    /// Получить описание последнего обновления
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetLastReleaseNotes()
    {
        var lv = await GetLastVersion();
        return lv?.ReleaseNotes;
    }

    /// <summary>
    /// Получить ссылку на файл скачивания последней версии
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetDownloadLink()
    {
        var lv = await GetLastVersion();
        return lv?.UpdateUrl;
    }

    private async Task<LastVersion> GetLastVersion()
    {
        if (_LastVersion != null) return _LastVersion;

        using (var client = new HttpClient())
        {
            var response = await client.GetAsync(LatestVersionTxtUrl);

            if (response.IsSuccessStatusCode)
            {
                var txt = await response.Content.ReadAsStringAsync();

                var lines = txt.Split(
                    new[] { "\r\n", "\r", "\n" },
                    StringSplitOptions.None);

                if (lines.Length > 2)
                {
                    _LastVersion = new LastVersion(lines[0], lines[1],
                        string.Join(Environment.NewLine, lines.Skip(2)));
                }

            }
        }

        return _LastVersion;
    }

    /// <summary> Показать окно обновления </summary>
    public void ShowUpdateWindow()
    {
        var wnd = _AppWindowsService.GetDialogWindow<UpdateWindow>();
        var vm = wnd.ViewModel;
        vm.DownloadLink = _LastVersion.UpdateUrl;
        vm.ReleaseNotes = _LastVersion.ReleaseNotes;
            
        wnd.ShowDialog();
    }

    private record LastVersion(string Version, string UpdateUrl, string ReleaseNotes);
}
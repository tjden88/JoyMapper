using System.Diagnostics;

namespace SharedServices;

/// <summary>
/// Сервис проверки обновлений
/// </summary>
public class AppUpdateService
{
    //private const string LatestVersionTxtUrl = @"https://raw.githubusercontent.com/tjden88/JoyMapper/v1.4/JoyMapper/LatestVersion.txt"; // Debug
    private const string LatestVersionTxtUrl = @"https://raw.githubusercontent.com/tjden88/JoyMapper/master/JoyMapper/LatestVersion.txt";

    private LastVersion _LastVersion;

    /// <summary> Путь к exe файлу программы </summary>
    public string ExeFilePath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "JoyMapper.exe");

    /// <summary>
    /// Получить последнюю версию маппера
    /// </summary>
    /// <returns>null, если программа не найдена</returns>
    public string GetCurrentAppVersion()
    {
        var file = ExeFilePath;
        if (!File.Exists(file))
            return null;

        var versionInfo = FileVersionInfo.GetVersionInfo(file);

        return versionInfo.FileVersion;
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
        var currVersion = new Version(GetCurrentAppVersion() ?? "1.0.0");

        var result = version.CompareTo(currVersion);

        return result > 0;
    }

    /// <summary>
    /// Получить последнюю релизную версию
    /// </summary>
    /// <returns></returns>
    public async Task<string> GetLastLatestVersion()
    {
        var lv = await GetLastVersion();
        return lv?.Version;
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

        try
        {
            using var client = new HttpClient();
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
        catch (Exception e)
        {
            Debug.WriteLine(e);
            return null;
        }

        return _LastVersion;
    }

    private record LastVersion(string Version, string UpdateUrl, string ReleaseNotes);
}
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace JoyMapper.Services
{
    /// <summary>
    /// Сервис проверки обновлений
    /// </summary>
    internal class UpdateChecker
    {

        private const string LatestVersionTxtUrl = @"https://github.com/tjden88/JoyMapper/tree/master/JoyMapper/LatestVersion.txt";

        private record LastVersion(string Version, string UpdateUrl, string ReleaseNotes);


        private LastVersion _LastVersion;


        /// <summary>
        /// Проверить, есть ли новая версия
        /// </summary>
        /// <param name="CurrentVersion">Текущая версия</param>
        /// <returns></returns>
        public async Task<bool> CheckUpdate(string CurrentVersion)
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

                    var splited = txt.Split(Environment.NewLine);
                    if (splited.Length > 2)
                    {
                        _LastVersion = new LastVersion(splited[0], splited[1],
                            string.Join(Environment.NewLine, splited.Skip(2)));
                    }

                }
            }

            return _LastVersion;
        }

    }
}

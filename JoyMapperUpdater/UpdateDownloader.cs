using SharedServices;

namespace JoyMapperUpdater
{
    internal class UpdateDownloader
    {
        private readonly AppUpdateService _Updater;

        public UpdateDownloader(AppUpdateService Updater)
        {
            _Updater = Updater;
        }

        public async Task DownloadUpdate()
        {
            Console.WriteLine($"Скачивание обновления. Актуальная версия программы: {await _Updater.GetLastLatestVersion()}");
            await Task.Delay(300);
            Console.WriteLine("Скачивание завершено. Установка...");
        }
    }
}

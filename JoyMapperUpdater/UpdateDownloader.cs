using SharedServices;
using System.IO.Compression;

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

            using var client = new HttpClient();
            var downloadLink = await _Updater.GetDownloadLink();
            var tempFile = Path.GetTempFileName();

            try
            {
                await using var app = await client.GetStreamAsync(downloadLink);
                await using var fileStream = File.OpenWrite(tempFile);
                await app.CopyToAsync(fileStream);
            }
            catch (Exception)
            {
                Console.WriteLine("Ошибка скачивания обновления. Попробуйте позже или скачайте обновление вручную. Ссылка:\n" +
                                  $"{downloadLink}\n" +
                                  "Нажимте любую кнопку для выхода.");
                Console.ReadKey();
                Environment.Exit(1);
            }

            Console.WriteLine("Скачивание завершено. Установка...");
            var directory = AppDomain.CurrentDomain.BaseDirectory;

            try
            {
                ZipFile.ExtractToDirectory(tempFile, directory, true);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Ошибка установки в папку {directory}:\n{e.Message}\n" +
                                  "Нажимте любую кнопку для выхода.");
                Console.ReadKey();
                Environment.Exit(2);
            }

            Console.WriteLine("Обновление успешно завершено. Нажимте любую кнопку для выхода.");
            Console.ReadKey();
            Environment.Exit(0);
        }
    }
}

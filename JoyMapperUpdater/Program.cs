using JoyMapperUpdater;
using SharedServices;
Console.WriteLine("Проверка обновлений программы...");

if (args.Length > 0)
    if (int.TryParse(args[0], out var delay))
        await Task.Delay(delay);

var updater = new AppUpdateService();
var downloader = new UpdateDownloader(updater);
var currentVersion = updater.GetCurrentAppVersion();

if (currentVersion is null)
{
    Console.WriteLine("Не удалось определить текущую версию программы. Скачать последнюю версию в эту папку?\n" +
                      "Y - да, N - выход");

    while (true)
    {
        var key = Console.ReadKey().Key;
        switch (key)
        {
            case ConsoleKey.Y:
                Console.WriteLine();
                await downloader.DownloadUpdate();
                return;
            case ConsoleKey.N:
                return;
            default:
                Console.WriteLine(" Неверная команда");
                break;
        }

    }
}

var checkUpdate = await updater.CheckUpdate();

if (!checkUpdate)
{
    Console.WriteLine("Обновлений не найдено.\nНажмите любую клавишу, чтобы закрыть это окно.");
    Console.ReadKey();
    return;
}

await downloader.DownloadUpdate();
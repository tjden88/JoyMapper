using JoyMapper.Models;
using JoyMapper.Views.Windows;

namespace JoyMapper.Services.Data;

/// <summary>
/// Создание, изменение модификаторов
/// </summary>
public class ModificatorManager
{
    private readonly AppWindowsService _AppWindowsService;

    public ModificatorManager(AppWindowsService AppWindowsService)
    {
        _AppWindowsService = AppWindowsService;
    }

    public Modificator AddModificator()
    {
        var wnd = _AppWindowsService.GetDialogWindow<EditModificator>();
        return wnd.ShowDialog() != true ? null : wnd.GetModel();
    }


    public Modificator UpdateModificator(Modificator modificator)
    {
        var wnd = _AppWindowsService.GetDialogWindow<EditModificator>();
        wnd.SetModel(modificator);
        return wnd.ShowDialog() != true ? null : wnd.GetModel();
    }

}
using JoyMapper.Models;
using JoyMapper.Views.Windows;

namespace JoyMapper.Services.Data;

/// <summary>
/// Создание, изменение и удаление паттернов
/// </summary>
public class JoyPatternManager
{
    private readonly AppWindowsService _AppWindowsService;

    public JoyPatternManager(AppWindowsService AppWindowsService)
    {
        _AppWindowsService = AppWindowsService;
    }

    public JoyPattern AddPattern()
    {
        var patternWindow = _AppWindowsService.GetDialogWindow<EditPattern>();

        if (patternWindow.ShowDialog() != true) return null;

        var pattern = patternWindow.GetModel();
        return pattern;
    }

    public JoyPattern EditPattern(JoyPattern Pattern)
    {
        var patternWindow = _AppWindowsService.GetDialogWindow<EditPattern>();
        patternWindow.SetModel(Pattern);

        return patternWindow.ShowDialog() != true
            ? null
            : patternWindow.GetModel();
    }

}
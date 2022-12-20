using JoyMapper.Models;
using JoyMapper.Views.Windows;

namespace JoyMapper.Services.Data;

/// <summary>
/// Создание, изменение и удаление паттернов
/// </summary>
public class JoyPatternManager
{
    private readonly AppWindowsService _AppWindowsService;
    private readonly DataSerializer _DataSerializer;

    public JoyPatternManager(AppWindowsService AppWindowsService, DataSerializer DataSerializer)
    {
        _AppWindowsService = AppWindowsService;
        _DataSerializer = DataSerializer;
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

    public JoyPattern CopyPattern(JoyPattern pattern) => 
        _DataSerializer.CopyObject(pattern);
}
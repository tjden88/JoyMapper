using System;
using System.IO;
using System.Text;
using System.Windows;
using Newtonsoft.Json;

namespace JoyMapper.Services;

/// <summary>
/// Методы для сохранения, загрузки и копирования объектов
/// </summary>
public class DataSerializer
{
    private readonly JsonSerializerSettings _Settings = new()
    {
        TypeNameHandling = TypeNameHandling.Auto,
        NullValueHandling = NullValueHandling.Ignore,
    };

    /// <summary>
    /// Создать копию объекта
    /// </summary>
    /// <param name="obj">Исходный объект</param>
    /// <returns>default, если не удалось</returns>
    public T CopyObject<T>(T obj)
    {
        try
        {
            var serialized = JsonConvert.SerializeObject(obj, Formatting.Indented, _Settings);
            return JsonConvert.DeserializeObject<T>(serialized, _Settings);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Ошибка копирования объекта");
            return default;
        }
    }

    /// <summary>
    /// Сохранить объект в файл
    /// </summary>
    /// <param name="obj">Исходный объект</param>
    /// <param name="FileName">Имя файла</param>
    public void SaveToFile<T>(T obj, string FileName)
    {
        try
        {
            var serialized = JsonConvert.SerializeObject(obj, Formatting.Indented, _Settings);

            File.WriteAllText(FileName, serialized, Encoding.UTF8);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Ошибка при сохраниении данных в файл");
        }
    }

    /// <summary>
    /// Загрузить из файла
    /// </summary>
    /// <param name="FileName">Имя файла</param>
    /// <returns>default, если не удалось</returns>
    public T LoadFromFile<T>(string FileName)
    {
        if (!File.Exists(FileName))
            return default;
        try
        {
            var jsonString = File.ReadAllText(FileName);
            return JsonConvert.DeserializeObject<T>(jsonString, _Settings);
        }
        catch (Exception e)
        {
            MessageBox.Show(e.Message, "Ошибка загрузки данных из файла");
            return default;
        }
    }
}
using System;
using System.IO;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Windows;

namespace JoyMapper.Services
{
    /// <summary>
    /// Методы для сохранения, загрузки и копирования объектов
    /// </summary>
    internal class DataSerializer
    {
        /// <summary>
        /// Создать копию объекта
        /// </summary>
        /// <param name="obj">Исходный объект</param>
        /// <returns>default, если не удалось</returns>
        public T CopyObject<T>(T obj)
        {
            try
            {
                var serialized = JsonSerializer.Serialize(obj);
                return JsonSerializer.Deserialize<T>(serialized);
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
                var options = new JsonSerializerOptions { WriteIndented = true, Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping, };
                var serialized = JsonSerializer.Serialize(obj, options);

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
                return JsonSerializer.Deserialize<T>(jsonString);
            }
            catch (Exception e)
            {
                MessageBox.Show(e.Message, "Ошибка загрузки данных из файла");
                return default;
            }
        }
    }

}

using System.Diagnostics.CodeAnalysis;

namespace JoyMapper.Interfaces
{
    /// <summary>
    /// Интерфейс, обозначающий, что данное представление предназначено для редактирования или создания новой сущности
    /// </summary>
    /// <typeparam name="TModel">Тип модели сущности</typeparam>
    public interface IEditModel<TModel>
    {
        /// <summary>
        /// Получить модель из представления
        /// </summary>
        TModel GetModel();


        /// <summary> Установить модель в представление для редактирования </summary>
        void SetModel([NotNull] TModel model);

    }
}

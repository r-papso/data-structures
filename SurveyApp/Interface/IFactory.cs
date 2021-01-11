using Structures.Interface;
using SurveyApp.Model;
using System.Collections.Generic;

namespace SurveyApp.Interface
{
    /// <summary>
    /// Factory design pattern, creating new instances of <see cref="ISerializable"/>
    /// </summary>
    public interface IFactory
    {
        /// <summary>
        /// Gets instance of serializable
        /// </summary>
        /// <returns>Instance of serializable</returns>
        public ISerializable GetSerializable();

        /// <summary>
        /// Gets instance of serializable with specified id and random property values
        /// </summary>
        /// <param name="id">ID of serializable</param>
        /// <returns>Instance of serializable with specified id and random property values</returns>
        public ISerializable GetSerializable(int id);

        /// <summary>
        /// Generates collection of serializables according to <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria">Criteria used for generate serializables</param>
        /// <returns>Collection of serializables</returns>
        public IEnumerable<ISerializable> GetSerializables(GenerationCriteria criteria);
    }
}

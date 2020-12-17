using Structures.Interface;
using SurveyApp.Adapter;
using SurveyApp.Model;

namespace SurveyApp.Interface
{
    /// <summary>
    /// Manages operations performed on <see cref="Localizable"/>
    /// </summary>
    public interface IManager
    {
        /// <summary>
        /// Localizables' collection
        /// </summary>
        public HashFileAdapter<ISerializable> Serializables { get; }

        /// <summary>
        /// Finds localizables by specified <paramref name="id"/>
        /// </summary>
        /// <param name="id">Localizable's id to be searched</param>
        public void Find(int id);

        /// <summary>
        /// Inserts a localizable
        /// </summary>
        /// <param name="data">Localizable to be inserted</param>
        public void Insert(ISerializable data);

        /// <summary>
        /// Updates a localizable
        /// </summary>
        /// <param name="oldData">Old localizable values</param>
        /// <param name="newData">New localizable values</param>
        public void Update(ISerializable oldData, ISerializable newData);

        /// <summary>
        /// Deletes a localizable
        /// </summary>
        /// <param name="id">Localizable's id to be deleted</param>
        public void Delete(int id);

        /// <summary>
        /// Generates localizables according to <paramref name="criteria"/>
        /// </summary>
        /// <param name="criteria">Criteria used for generate localizables</param>
        public void Generate(GenerationCriteria criteria);

        /// <summary>
        /// Initializes new database at specified directory
        /// </summary>
        /// <param name="directory">Directory where the database will be created</param>
        /// <param name="clusterSize">File system's cluster size in bytes</param>
        public void NewDatabase(string directory, int clusterSize);

        /// <summary>
        /// Loads existing database from specified directory
        /// </summary>
        /// <param name="directory">Directory from where database will be loaded</param>
        public void LoadDatabase(string directory);

        /// <summary>
        /// Releases all resources held by database
        /// </summary>
        public void Release();

        /// <summary>
        /// Gets instance of localizable
        /// </summary>
        /// <returns>Instance of localizable</returns>
        public ISerializable GetSerializable();

        /// <summary>
        /// Gets instance of localizable with specified id and random property values
        /// </summary>
        /// <param name="id">ID of localizable</param>
        /// <returns>Instance of localizable with specified id and random property values</returns>
        public ISerializable GenerateSerializable(int id);
    }
}

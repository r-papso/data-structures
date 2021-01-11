using Structures.Interface;
using SurveyApp.Adapter;
using SurveyApp.Interface;
using SurveyApp.Model;

namespace SurveyApp.Service
{
    public class Manager : IManager
    {
        private readonly IFactory _factory;

        public HashFileAdapter<ISerializable> Serializables { get; }

        public Manager(IFactory factory)
        {
            _factory = factory;
            Serializables = new HashFileAdapter<ISerializable>();
        }

        public void Find(int id) => Serializables.Find(_factory.GetSerializable(id));

        public void Insert(ISerializable data) => Serializables.Insert(data);

        public void Update(ISerializable oldData, ISerializable newData) => Serializables.Update(oldData, newData);

        public void Delete(int id) => Serializables.Delete(_factory.GetSerializable(id));

        public void FillDatabase(GenerationCriteria criteria) => Serializables.Fill(_factory.GetSerializables(criteria));

        public void NewDatabase(string directory, int clusterSize) => Serializables.New(directory, clusterSize, _factory.GetSerializable());

        public void LoadDatabase(string directory) => Serializables.Load(directory, _factory.GetSerializable());

        public void Release() => Serializables.Release();
    }
}

using Structures;
using Structures.Interface;
using SurveyApp.Adapter;
using SurveyApp.Interface;
using SurveyApp.Model;
using System;
using System.Collections.Generic;

namespace SurveyApp.Service
{
    public abstract class Manager : IManager
    {
        public HashFileAdapter<ISerializable> Serializables { get; }

        public Manager() => Serializables = new HashFileAdapter<ISerializable>();

        public void Find(int id) => Serializables.Find(GenerateSerializable(id));

        public void Insert(ISerializable data) => Serializables.Insert(data);

        public void Update(ISerializable oldData, ISerializable newData) => Serializables.Update(oldData, newData);

        public void Delete(int id) => Serializables.Delete(GenerateSerializable(id));

        public void Generate(GenerationCriteria criteria) => Serializables.Generate(GenerateLocalizables(criteria));

        public void NewDatabase(string directory, int clusterSize) => Serializables.New(directory, clusterSize, GetSerializable());

        public void LoadDatabase(string directory) => Serializables.Load(directory, GetSerializable());

        public void Release() => Serializables.Release();

        private IEnumerable<ISerializable> GenerateLocalizables(GenerationCriteria criteria)
        {
            var collection = new List<ISerializable>();
            var randId = new Random();
            var usedIds = StructureFactory.Instance.GetHashSet<int>();

            for (int i = 0; i < criteria.LocationsCount; i++)
            {
                var id = i;

                if (criteria.RandomIds)
                {
                    id = randId.Next();

                    while (usedIds.Find(id).Count != 0)
                        id = randId.Next();

                    usedIds.Insert(id);
                }

                collection.Add(GenerateSerializable(id));
            }

            return collection;
        }

        public abstract ISerializable GetSerializable();

        public abstract ISerializable GenerateSerializable(int id);
    }
}

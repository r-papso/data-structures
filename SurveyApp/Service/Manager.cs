using Structures;
using Structures.Interface;
using SurveyApp.Adapter;
using SurveyApp.Interface;
using SurveyApp.Model;
using System;
using System.Collections.Generic;

namespace SurveyApp.Service
{
    public abstract class Manager<T> : IManager<T> where T : ISerializable, ILocalizable, new()
    {
        public HashFileAdapter<T> Localizables { get; private set; }

        public Manager() => Localizables = new HashFileAdapter<T>();

        public void Find(int id)
        {
            var loc = new T() { ID = id };
            Localizables.Find(loc);
        }

        public void Insert(T location) => Localizables.Insert(location);

        public void Update(T oldLocation, T newLocation) => Localizables.Update(oldLocation, newLocation);

        public void Delete(int id) => Localizables.Delete(new T() { ID = id });

        public void Generate(GenerationCriteria criteria) => Localizables.Generate(GenerateLocalizables(criteria));

        public void NewDatabase(string directory, int clusterSize) => Localizables.New(directory, clusterSize);

        public void LoadDatabase(string directory) => Localizables.Load(directory);

        public void Release() => Localizables.Release();

        private IEnumerable<T> GenerateLocalizables(GenerationCriteria criteria)
        {
            var locations = new List<T>();
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

                locations.Add(GenerateLocalizable(id));
            }

            return locations;
        }

        public abstract T GetLocalizable();

        public abstract T GenerateLocalizable(int locId);
    }
}

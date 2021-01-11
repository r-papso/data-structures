using Structures;
using Structures.Interface;
using SurveyApp.Interface;
using SurveyApp.Model;
using System;
using System.Collections.Generic;

namespace SurveyApp.Service
{
    public abstract class Factory : IFactory
    {
        public abstract ISerializable GetSerializable();

        public abstract ISerializable GetSerializable(int id);

        public IEnumerable<ISerializable> GetSerializables(GenerationCriteria criteria)
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

                collection.Add(GetSerializable(id));
            }

            return collection;
        }
    }
}

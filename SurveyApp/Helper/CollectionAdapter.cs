﻿using Structures;
using Structures.Tree;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;

namespace SurveyApp.Helper
{
    public class CollectionAdapter<T> : INotifyCollectionChanged, IEnumerable<T> where T : IKdComparable, ISaveable, new()
    {
        private T _lastUpper;
        private T _lastLower;

        public IBSPTree<T> Tree { get; private set; }

        public IEnumerable<T> Found { get; private set; }

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CollectionAdapter() => Tree = StructureFactory.Instance.GetBSPTree<T>();

        public CollectionAdapter(IBSPTree<T> tree) => Tree = tree;

        public ICollection<T> Get(T data) => Tree.Find(data);

        public ICollection<T> Get(T lowerBound, T upperBound) => Tree.Find(lowerBound, upperBound);

        public void Find(T data)
        {
            (_lastLower, _lastUpper) = (data, data);

            Found = Tree.Find(data);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public void Find(T lowerBound, T upperBound)
        {
            (_lastLower, _lastUpper) = (lowerBound, upperBound);

            Found = Tree.Find(lowerBound, upperBound);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public void SetEmptyFound()
        {
            Found = Enumerable.Empty<T>();

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public void Insert(T data)
        {
            Tree.Insert(data);

            if (Found != null)
            {
                Found = Tree.Find(_lastLower, _lastUpper);
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
                OnCollectionChanged(args);
            }
            else
            {
                var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Add, data);
                OnCollectionChanged(args);
            }
        }

        public void Update(T oldData, T newData)
        {
            Tree.Update(oldData, newData);

            if (Found != null)
                Found = Tree.Find(_lastLower, _lastUpper);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public void Delete(T data)
        {
            Tree.Delete(data);

            if (Found != null)
                Found = Tree.Find(_lastLower, _lastUpper);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public void Reset()
        {
            Found = null;
            (_lastLower, _lastUpper) = (default, default);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public void Generate(IEnumerable<T> data)
        {
            Tree = StructureFactory.Instance.GetBSPTree(data);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public void Save(string filePath) => Tree.Save(filePath);

        public void Load(string filePath)
        {
            Tree = StructureFactory.Instance.GetBSPTree<T>();
            Tree.Load(filePath);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (Found != null)
                return Found.GetEnumerator();
            else
                return Tree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
    }
}
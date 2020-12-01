using Structures;
using Structures.Interface;
using SurveyApp.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace SurveyApp.Adapter
{
    /// <summary>
    /// Adapter used to add <see cref="INotifyCollectionChanged"/> and <see cref="ISaveable"/> behavior to <see cref="ITree{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of elements in <see cref="ITree{T}"/></typeparam>
    public class TreeAdapter<T> : INotifyCollectionChanged, IEnumerable<T> where T : IKdComparable, ISaveable, new()
    {
        private static readonly string _csvDelimiter = ";";

        private T _lastUpper;
        private T _lastLower;

        /// <summary>
        /// <see cref="ITree{T}"/>
        /// </summary>
        public ITree<T> Tree { get; private set; }

        /// <summary>
        /// Elements found by <see cref="Find(T, T)"/> method
        /// </summary>
        public IEnumerable<T> Found { get; private set; }

        /// <summary>
        /// Event invoked when either <see cref="ITree{T}"/> or <see cref="Found"/> is changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public TreeAdapter() => Tree = StructureFactory.Instance.GetKdTree<T>();

        /// <summary>
        /// Constructs <see cref="TreeAdapter{T}"/> with specified <see cref="ITree{T}"/>
        /// </summary>
        /// <param name="tree"><see cref="ITree{T}"/></param>
        public TreeAdapter(ITree<T> tree) => Tree = tree;

        /// <summary>
        /// Gets all occurences of <paramref name="data"/> returned by <see cref="ITree{T}.Find(T)"/>
        /// </summary>
        /// <param name="data">Data to be found</param>
        /// <returns>All occurences of <paramref name="data"/></returns>
        public ICollection<T> Get(T data) => Tree.Find(data);

        /// <summary>
        /// Gets all occurences between <paramref name="lowerBound"/> and <paramref name="upperBound"/> returned by <see cref="ITree{T}.Find(T, T)"/>
        /// </summary>
        /// <param name="lowerBound">Lower bound</param>
        /// <param name="upperBound">Upper bound</param>
        /// <returns>All occurences between <paramref name="lowerBound"/> and <paramref name="upperBound"/></returns>
        public ICollection<T> Get(T lowerBound, T upperBound) => Tree.Find(lowerBound, upperBound);

        /// <summary>
        /// Finds all occurences of <paramref name="data"/> and stores them in <see cref="Found"/> property
        /// </summary>
        /// <param name="data">Data to be found</param>
        public void Find(T data)
        {
            (_lastLower, _lastUpper) = (data, data);

            Found = Tree.Find(data);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Finds all occurences between <paramref name="lowerBound"/> and <paramref name="upperBound"/> and stores them in <see cref="Found"/> property
        /// </summary>
        /// <param name="lowerBound">Lower bound</param>
        /// <param name="upperBound">Upper bound</param>
        public void Find(T lowerBound, T upperBound)
        {
            (_lastLower, _lastUpper) = (lowerBound, upperBound);

            Found = Tree.Find(lowerBound, upperBound);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Sets <see cref="Found"/> property to null and invokes <see cref="CollectionChanged"/> event
        /// </summary>
        public void SetEmptyFound()
        {
            Found = Enumerable.Empty<T>();
            (_lastLower, _lastUpper) = (default, default);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Inserts <paramref name="data"/> to <see cref="Tree"/> and invokes <see cref="CollectionChanged"/> event
        /// </summary>
        /// <param name="data">Data to be inserted into <see cref="Tree"/></param>
        public void Insert(T data)
        {
            Tree.Insert(data);

            if (Found != null && _lastLower != null && _lastUpper != null)
                Found = Tree.Find(_lastLower, _lastUpper);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Updates values of <paramref name="oldData"/> element located in <see cref="Tree"/> to 
        /// <paramref name="newData"/> values and invokes <see cref="CollectionChanged"/> event
        /// </summary>
        /// <param name="oldData">Element <see cref="IKdComparable.Identical(IKdComparable)"/> to updating element</param>
        /// <param name="newData">New element values</param>
        public void Update(T oldData, T newData)
        {
            Tree.Update(oldData, newData);

            if (Found != null && _lastLower != null && _lastUpper != null)
                Found = Tree.Find(_lastLower, _lastUpper);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Removes element from <see cref="Tree"/> and invokes <see cref="CollectionChanged"/> event
        /// </summary>
        /// <param name="data">Element <see cref="IKdComparable.Identical(IKdComparable)"/> to removing element</param> and invokes <see cref="CollectionChanged"/> event
        public void Delete(T data)
        {
            Tree.Delete(data);

            if (Found != null && _lastLower != null && _lastUpper != null)
                Found = Tree.Find(_lastLower, _lastUpper);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Sets <see cref="Found"/> to null and invokes <see cref="CollectionChanged"/> event
        /// </summary>
        public void Reset()
        {
            Found = null;
            (_lastLower, _lastUpper) = (default, default);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Creates new <see cref="ITree{T}"/> from <paramref name="data"/> and invokes <see cref="CollectionChanged"/> event
        /// </summary>
        /// <param name="data">Data used in tree construction</param>
        public void Generate(IEnumerable<T> data)
        {
            Tree = StructureFactory.Instance.GetKdTree(data);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Saves <see cref="ITree{T}"/> to CSV file
        /// </summary>
        /// <param name="filePath">Path of file, should have .csv extension</param>
        public void Save(string filePath)
        {
            using var writer = new StreamWriter(filePath);

            foreach (var location in Tree.LevelOrderTraversal)
            {
                writer.WriteLine(location.ToCsv(_csvDelimiter));
            }
        }

        /// <summary>
        /// Loads <see cref="ITree{T}"/> from CSV file
        /// </summary>
        /// <param name="filePath">Path of file, has to be file generated by <see cref="Save(string)"/> method</param>
        public void Load(string filePath)
        {
            Tree = StructureFactory.Instance.GetKdTree<T>();
            using var reader = new StreamReader(filePath);

            string line;
            while ((line = reader.ReadLine()) != null)
            {
                var data = new T();
                data.FromCsv(line, _csvDelimiter);
                Tree.Insert(data);
            }

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Returns enumerator of <see cref="Found"/> if not null, otherwise returns enumerator of <see cref="Tree"/>
        /// </summary>
        /// <returns>Enumerator of either <see cref="Found"/> or <see cref="Tree"/></returns>
        public IEnumerator<T> GetEnumerator()
        {
            if (Found != null)
                return Found.GetEnumerator();
            else
                return Tree.GetEnumerator();
        }

        /// <summary>
        /// Calls <see cref="GetEnumerator"/> method
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void OnCollectionChanged(NotifyCollectionChangedEventArgs args)
        {
            CollectionChanged?.Invoke(this, args);
        }
    }
}

using Structures;
using Structures.Interface;
using SurveyApp.Interface;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;

namespace SurveyApp.Helper
{
    /// <summary>
    /// Adapter used to add <see cref="INotifyCollectionChanged"/> and <see cref="ISaveable"/> behavior to <see cref="IBSPTree{T}"/>
    /// </summary>
    /// <typeparam name="T">Type of elements in <see cref="IBSPTree{T}"/></typeparam>
    public class CollectionAdapter<T> : INotifyCollectionChanged, IEnumerable<T> where T : IKdComparable, ISaveable, new()
    {
        private static string _csvDelimiter = ";";

        private T _lastUpper;
        private T _lastLower;

        /// <summary>
        /// <see cref="IBSPTree{T}"/>
        /// </summary>
        public IBSPTree<T> Tree { get; private set; }

        /// <summary>
        /// Elements found by <see cref="Find(T, T)"/> method
        /// </summary>
        public IEnumerable<T> Found { get; private set; }

        /// <summary>
        /// Event invoked when either <see cref="IBSPTree{T}"/> or <see cref="Found"/> is changed
        /// </summary>
        public event NotifyCollectionChangedEventHandler CollectionChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public CollectionAdapter() => Tree = StructureFactory.Instance.GetBSPTree<T>();

        /// <summary>
        /// Constructs <see cref="CollectionAdapter{T}"/> with specified <see cref="IBSPTree{T}"/>
        /// </summary>
        /// <param name="tree"><see cref="IBSPTree{T}"/></param>
        public CollectionAdapter(IBSPTree<T> tree) => Tree = tree;

        /// <summary>
        /// Gets all occurences of <paramref name="data"/> returned by <see cref="IBSPTree{T}.Find(T)"/>
        /// </summary>
        /// <param name="data">Data to be found</param>
        /// <returns>All occurences of <paramref name="data"/></returns>
        public ICollection<T> Get(T data) => Tree.Find(data);

        /// <summary>
        /// Gets all occurences between <paramref name="lowerBound"/> and <paramref name="upperBound"/> returned by <see cref="IBSPTree{T}.Find(T, T)"/>
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

            if (Found != null)
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

            if (Found != null)
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

            if (Found != null)
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
        /// Creates new <see cref="IBSPTree{T}"/> from <paramref name="data"/> and invokes <see cref="CollectionChanged"/> event
        /// </summary>
        /// <param name="data">Data used in tree construction</param>
        public void Generate(IEnumerable<T> data)
        {
            Tree = StructureFactory.Instance.GetBSPTree(data);

            var args = new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset);
            OnCollectionChanged(args);
        }

        /// <summary>
        /// Saves <see cref="IBSPTree{T}"/> to CSV file
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
        /// Loads <see cref="IBSPTree{T}"/> from CSV file
        /// </summary>
        /// <param name="filePath">Path of file, has to be file generated by <see cref="Save(string)"/> method</param>
        public void Load(string filePath)
        {
            Tree = StructureFactory.Instance.GetBSPTree<T>();
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

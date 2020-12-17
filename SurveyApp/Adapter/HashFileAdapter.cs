using Structures;
using Structures.Interface;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SurveyApp.Adapter
{
    /// <summary>
    /// Adapter encapsulating <see cref="IHashFile{T}"/> used in data binding and visualisation of this structure
    /// </summary>
    /// <typeparam name="T">Type of elements stored at <see cref="IHashFile{T}"/></typeparam>
    public class HashFileAdapter<T> : INotifyPropertyChanged where T : ISerializable
    {
        private IHashFile<T> _struct;
        private IEnumerable<T> _found;

        /// <summary>
        /// Element found by <see cref="Find(T)"/>
        /// </summary>
        public IEnumerable<T> Found
        {
            get => _found;
            private set
            {
                _found = value;
                OnPropertyChanged();
            }
        }

        /// <summary>
        /// Enumerable used to traverse through structure's primary file
        /// </summary>
        public IEnumerable<IBlockState<T>> PrimaryFile => _struct?.PrimaryFileState ?? null;

        /// <summary>
        /// Enumerable used to traverse through primary file's free addresses
        /// </summary>
        public IEnumerable<long> PrimaryFileFreeAddresses => _struct?.PrimaryFileFreeBlocks ?? null;

        /// <summary>
        /// Enumerable used to traverse through structure's overflow file
        /// </summary>
        public IEnumerable<IBlockState<T>> OverflowFile => _struct?.OverflowFileState ?? null;

        /// <summary>
        /// Enumerable used to traverse through overflow file's free addresses
        /// </summary>
        public IEnumerable<long> OverflowFileFreeAddresses => _struct?.OverflowFileFreeBlocks ?? null;

        /// <summary>
        /// Event invoked when some action over this adapter has occurred
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Default constructor
        /// </summary>
        public HashFileAdapter()
        { }

        /// <summary>
        /// Finds element in <see cref="IHashFile{T}"/> equals to <paramref name="data"/>
        /// </summary>
        /// <param name="data">Element to be found</param>
        public void Find(T data) => Found = _struct.Find(data);

        /// <summary>
        /// Inserts <paramref name="data"/> into <see cref="IHashFile{T}"/>
        /// </summary>
        /// <param name="data">Element to be inserted</param>
        public void Insert(T data)
        {
            _struct.Insert(data);
            FilesChanged();
        }

        /// <summary>
        /// Updates values of <paramref name="oldData"/> to <paramref name="newData"/> values
        /// </summary>
        /// <param name="oldData">Element to be updated</param>
        /// <param name="newData">New values of updating element</param>
        public void Update(T oldData, T newData)
        {
            _struct.Update(oldData, newData);
            FilesChanged();
        }

        /// <summary>
        /// Removes <paramref name="data"/> from <see cref="IHashFile{T}"/>
        /// </summary>
        /// <param name="data">Element to be removed</param>
        public void Delete(T data)
        {
            _struct.Delete(data);
            FilesChanged();
        }

        /// <summary>
        /// Fills <see cref="IHashFile{T}"/> with elements of <paramref name="collection"/>
        /// </summary>
        /// <param name="collection">Elements to be inserted into <see cref="IHashFile{T}"/></param>
        public void Generate(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                _struct.Insert(item);
            }

            FilesChanged();
        }

        /// <summary>
        /// Loads <see cref="IHashFile{T}"/> from specified directory
        /// </summary>
        /// <param name="directory">Directory where files of <see cref="IHashFile{T}"/> are stored</param>
        public void Load(string directory, T prototype)
        {
            _struct = StructureFactory.Instance.GetExtendibleHashing<T>(directory, prototype);
            FilesChanged();
        }

        /// <summary>
        /// Creates new instance of <see cref="IHashFile{T}"/> in specific directory
        /// </summary>
        /// <param name="directory">Directory where <see cref="IHashFile{T}"/> will be created</param>
        /// <param name="clusterSize">File system's cluster size in bytes</param>
        public void New(string directory, int clusterSize, T prototype)
        {
            _struct = StructureFactory.Instance.GetExtendibleHashing<T>(directory, clusterSize, prototype);
            FilesChanged();
        }

        /// <summary>
        /// Releases all resources held by <see cref="IHashFile{T}"/>
        /// </summary>
        public void Release() => _struct?.Dispose();

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void FilesChanged()
        {
            OnPropertyChanged(nameof(PrimaryFile));
            OnPropertyChanged(nameof(OverflowFile));
            OnPropertyChanged(nameof(PrimaryFileFreeAddresses));
            OnPropertyChanged(nameof(OverflowFileFreeAddresses));
        }
    }
}

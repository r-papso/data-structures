using Structures;
using Structures.Interface;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace SurveyApp.Adapter
{
    public class FileStructureAdapter<T> : INotifyPropertyChanged where T : ISerializable, new()
    {
        private IFileStructure<T> _struct;
        private IEnumerable<T> _found;

        public IEnumerable<T> Found
        {
            get => _found;
            private set
            {
                _found = value;
                OnPropertyChanged();
            }
        }

        public ObservableCollection<T> Test { get; private set; }

        public IEnumerable<IBlockState<T>> PrimaryFile => _struct?.PrimaryFileState ?? null;

        public IEnumerable<IBlockState<T>> OverflowFile => _struct?.OverflowFileState ?? null;

        public event PropertyChangedEventHandler PropertyChanged;

        public FileStructureAdapter()
        { }

        public void Find(T data) => Found = _struct.Find(data);

        public void Insert(T data)
        {
            _struct.Insert(data);
            FilesChanged();
        }

        public void Update(T oldData, T newData)
        {
            _struct.Update(oldData, newData);
            FilesChanged();
        }

        public void Delete(T data)
        {
            _struct.Delete(data);
            FilesChanged();
        }

        public void Generate(IEnumerable<T> collection)
        {
            foreach (var item in collection)
            {
                _struct.Insert(item);
            }

            FilesChanged();
        }

        public void Load(string directory)
        {
            _struct = _struct = StructureFactory.Instance.GetExtendibleHashing<T>(directory);
            FilesChanged();
        }

        public void New(string directory, int clusterSize)
        {
            _struct = StructureFactory.Instance.GetExtendibleHashing<T>(directory, clusterSize);
            FilesChanged();
        }

        public void Release() => _struct?.Dispose();

        protected void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        private void FilesChanged()
        {
            OnPropertyChanged(nameof(PrimaryFile));
            OnPropertyChanged(nameof(OverflowFile));
        }
    }
}

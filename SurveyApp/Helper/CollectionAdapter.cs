using Structures;
using Structures.Tree;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

namespace SurveyApp.Helper
{
    public class CollectionAdapter<T> : INotifyCollectionChanged, IEnumerable<T> where T : IKdComparable
    {
        private IBSPTree<T> _tree;
        private IEnumerable<T> _found;

        public event NotifyCollectionChangedEventHandler CollectionChanged;

        public CollectionAdapter() => _tree = StructureFactory.Instance.GetBSPTree<T>();

        public CollectionAdapter(IBSPTree<T> tree) => _tree = tree;

        public ICollection<T> Get(T data) => _tree.Find(data);

        public ICollection<T> Get(T lowerBound, T upperBound) => _tree.Find(lowerBound, upperBound);

        public void Find(T data)
        {
            _found = _tree.Find(data);
            OnCollectionChanged();
        }

        public void Find(T lowerBound, T upperBound)
        {
            _found = _tree.Find(lowerBound, upperBound);
            OnCollectionChanged();
        }

        public void Insert(T data)
        {
            _tree.Insert(data);

            if (_found == null)
            {
                OnCollectionChanged();
            }
        }

        public void Update(T oldData, T newData)
        {
            _tree.Update(oldData, newData);

            if (_found == null)
            {
                OnCollectionChanged();
            }
        }

        public void Delete(T data)
        {
            _tree.Delete(data);

            if (_found == null)
            {
                OnCollectionChanged();
            }
        }

        public void Reset()
        {
            _found = null;
            OnCollectionChanged();
        }

        public void Generate(IEnumerable<T> data)
        {
            _tree = StructureFactory.Instance.GetBSPTree(data);
            OnCollectionChanged();
        }

        public IEnumerator<T> GetEnumerator()
        {
            if (_found != null)
                return _found.GetEnumerator();
            else
                return _tree.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

        protected void OnCollectionChanged()
        {
            CollectionChanged?.Invoke(this, new NotifyCollectionChangedEventArgs(NotifyCollectionChangedAction.Reset));
        }
    }
}

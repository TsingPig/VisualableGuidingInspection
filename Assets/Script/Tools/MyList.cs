using System;
using System.Collections.Generic;

namespace TsingPigSDK
{
    public class MyList<T> : List<T> where T : class
    {
        private List<T> _innerList = new List<T>();

        public event Action<T> OnItemAdded_Event;

        public event Action<T> OnItemRemoved_Event;
        public new int Count => _innerList.Count;

        public new T this[int idx]
        {
            get => _innerList[idx];
            set => _innerList[idx] = value;
        }

        public void Add(T item, Action callback = null)
        {
            _innerList.Add(item);
            OnItemAdded_Event?.Invoke(item);
            callback?.Invoke();
        }

        public void Remove(T item, Action callback = null)
        {
            _innerList.Remove(item);
            OnItemRemoved_Event?.Invoke(item);
            callback?.Invoke();
        }

        public T Pop()
        {
            if (_innerList.Count > 0)
            {
                T backItem = _innerList[_innerList.Count - 1];
                OnItemRemoved_Event?.Invoke(backItem);
                _innerList.RemoveAt(_innerList.Count - 1);
                return backItem;
            }
            else
            {
                Log.Warning("MyList长度为0，不能Pop");
                return default(T);
            }
        }

        public new void Clear()
        {
            Log.Info($"清空，长度：{_innerList.Count}");
            foreach (var item in _innerList)
            {
                OnItemRemoved_Event?.Invoke(item);
            }
            _innerList.Clear();
        }
        public List<T> GetList()
        {
            return _innerList;
        }
    }
}

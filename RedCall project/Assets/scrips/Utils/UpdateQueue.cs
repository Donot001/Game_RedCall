using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace RedCallFrame
{
    public sealed class UpdateQueue<T> where T : IGUpdateListener
    {
        private CachedLinkedList<T> _innerList = new CachedLinkedList<T>();

        public void Update(float deltaTime)
        {
            foreach (T updateListener in _innerList)
            {
                updateListener.FrameUpdate(deltaTime);
            }

            _innerList.CacheApply();
        }

        public void Add(T listener)
        {
            _innerList.Add(listener);
        }

        public void Remove(T listener)
        {
            _innerList.Remove(listener);
        }
    }
}
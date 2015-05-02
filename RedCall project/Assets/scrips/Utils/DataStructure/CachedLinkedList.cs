using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class CachedLinkedList<T> : IEnumerable<T>
{
    private LinkedList<T> _activeQueue = new LinkedList<T>();

    private LinkedList<T> _addedCache = new LinkedList<T>();
    private LinkedList<T> _deletedCache = new LinkedList<T>();

    public void CacheApply()
    {
        foreach (T element in _addedCache)
        {
            _activeQueue.AddLast(element);
        }
        _addedCache.Clear();

        foreach (T element in _deletedCache)
        {
            _activeQueue.Remove(element);
        }
        _deletedCache.Clear();
    }

    public void Add(T element)
    {
        if (!_activeQueue.Contains(element))
        {
            _addedCache.AddLast(element);
        }
        else
        {
            if (_deletedCache.Contains(element))
            {
                _deletedCache.Remove(element);
            }
        }
    }

    public void Remove(T element)
    {
        if (_activeQueue.Contains(element))
        {
            _deletedCache.AddLast(element);
        }
        else
        {
            if (_addedCache.Contains(element))
            {
                _addedCache.Remove(element);
            }
        }
    }

    public bool Contains(T element)
    {
        if (_deletedCache.Contains(element))
            return false;
        else
        {
            return _activeQueue.Contains(element);
        }
    }

    public void Clear()
    {
        _activeQueue.Clear();
        _addedCache.Clear();
        _deletedCache.Clear();
    }

    public IEnumerator<T> GetEnumerator()
    {
        return _activeQueue.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}
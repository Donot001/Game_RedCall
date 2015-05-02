using UnityEngine;
using System.Collections.Generic;

namespace RedCallFrame
{

    public delegate void EventListener(GameEvent e);

    public class EventDispatcher
    {
        private Dictionary<string, EventDispatcherItem> listeners;
        private static EventDispatcher instance;

        public static EventDispatcher GetInstance()
        {
            if (instance == null)
            {
                instance = new EventDispatcher();
            }
            return instance;
        }

        public EventDispatcher()
        {
            listeners = new Dictionary<string, EventDispatcherItem>();
        }

        public void Dispatch(GameEvent e)
        {
            //			Debug.Log("EventDispatcher.Dispatch " + e.Type);

            EventDispatcherItem item;
            if (listeners.TryGetValue(e.Type, out item))
            {
                item.Dispatch(e);
            }
        }

        public void AddListener(string eventType, EventListener listener, Object target)
        {
            //			Debug.Log("EventDispatcher.AddListener " + eventType);

            EventDispatcherItem item;
            if (!listeners.TryGetValue(eventType, out item))
            {
                item = new EventDispatcherItem();
                listeners.Add(eventType, item);
            }
            item.AddListener(listener, target);
        }

        public void AddListener(string eventType, EventListener listener)
        {
            AddListener(eventType, listener, null);
        }

        public void RemoveListener(string eventType, EventListener listener, Object target)
        {
            EventDispatcherItem item;
            if (listeners.TryGetValue(eventType, out item))
            {
                item.RemoveListener(listener, target);
            }
        }

        public void RemoveListener(string eventType, EventListener listener)
        {
            RemoveListener(eventType, listener, null);
        }

        public void RemoveListener(string eventType, Object target)
        {
            EventDispatcherItem item;
            if (listeners.TryGetValue(eventType, out item))
            {
                item.RemoveListener(target);
            }
        }
    }

    class EventDispatcherItem
    {
        class Item
        {
            public EventListener listener;
            public Object target;
        }

        private List<Item> backItemList;
        private List<Item> itemList;
        private bool itemChanged;

        public EventDispatcherItem()
        {
            backItemList = new List<Item>();
            itemList = new List<Item>();
            itemChanged = false;
        }

        public void Dispatch(GameEvent e)
        {
            if (itemChanged)
            {
                itemChanged = false;
                itemList.Clear();
                foreach (Item item in backItemList)
                {
                    itemList.Add(item);
                }
            }

            Object target = e.Target;
            if (target is GameObject)
            {
                GameObject gameObject = (GameObject)target;
                while (gameObject != null)
                {
                    bool found = false;
                    for (int i = itemList.Count - 1; i >= 0; --i)
                    {
                        Item item = itemList[i];
                        if (item.target == null || item.target == gameObject)
                        {
                            item.listener(e);
                            found = (item.target == gameObject);
                        }
                    }
                    if (found)
                    {
                        break;
                    }
                    if (gameObject.transform.parent != null)
                    {
                        gameObject = gameObject.transform.parent.gameObject;
                    }
                    else
                    {
                        gameObject = null;
                    }
                }
            }
            else
            {
                for (int i = itemList.Count - 1; i >= 0; --i)
                {
                    Item item = itemList[i];
                    if (item.target == null
                        || item.target == target)
                    {
                        item.listener(e);
                    }
                }
            }
        }

        public void AddListener(EventListener listener, Object target)
        {
            if (FindListener(listener, target) >= 0)
            {
                return;
            }

            Item item = new Item();
            item.listener = listener;
            item.target = target;

            itemChanged = true;
            backItemList.Add(item);
        }

        public void RemoveListener(EventListener listener, Object target)
        {
            for (; ; )
            {
                int index = FindListener(listener, target);
                if (index < 0)
                {
                    break;
                }
                itemChanged = true;
                backItemList.RemoveAt(index);
            }
        }

        public void RemoveListener(Object target)
        {
            int index = backItemList.Count - 1;
            for (; index >= 0; --index)
            {
                if (backItemList[index].target == target)
                {
                    itemChanged = true;
                    backItemList.RemoveAt(index);
                }
            }
        }

        public int FindListener(EventListener listener, Object target)
        {
            int index = -1;
            foreach (Item i in backItemList)
            {
                ++index;
                if (i.listener == listener
                    && i.target == target)
                {
                    return index;
                }
            }
            return -1;
        }

    }

}

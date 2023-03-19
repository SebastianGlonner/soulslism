using System;
using System.Collections.Generic;
using System.Threading;

namespace Soulslism
{
    public class EventManaged<T>
    {
        private struct EventMemoryPair<T>
        {
            public uint attachedId;
            public Event<T> eventHandler;
        }

        private Dictionary<uint, EventMemoryPair<T>> attachedEvents = new Dictionary<uint, EventMemoryPair<T>>();

        public bool AddTo(Event<T> eventHandler, Action<T> method)
        {
            uint eventId = eventHandler.UniqueId;
            if (attachedEvents.ContainsKey(eventId))
            {
                return false;
            }

            EventMemoryPair<T> eventPair = new EventMemoryPair<T>
            {
                attachedId = eventHandler.Add(method),
                eventHandler = eventHandler
            };

            attachedEvents.Add(eventId, eventPair);
            return true;
        }

        public void RemoveFrom(Event<T> eventHandler)
        {
            uint eventId = eventHandler.UniqueId;
            EventMemoryPair<T> eventPair;
            if (attachedEvents.TryGetValue(eventId, out eventPair))
            {
                eventHandler.Remove(eventPair.attachedId);
                attachedEvents.Remove(eventId);
            }
        }

        public void RemoveAll()
        {
            foreach (KeyValuePair<uint, EventMemoryPair<T>> kvp in attachedEvents)
            {
                EventMemoryPair<T> eventPair = kvp.Value;
                eventPair.eventHandler.Remove(eventPair.attachedId);
            }
        }
    }

    public class Event<T>
    {
        static private uint staticUniqueId = 0;
        static private uint GetUniqueId()
        {
            staticUniqueId++;
            return staticUniqueId;
        }

        protected uint listenerIds = 0;

        protected Dictionary<uint, Action<T>> listener;

        private readonly uint uniqueId;

        public Event()
        {
            uniqueId = Event<T>.GetUniqueId();
            listener = new Dictionary<uint, Action<T>>();
        }

        public virtual uint Add(Action<T> listener)
        {
            //Logging.Log("Event Add         : " + uniqueId);

            uint nextId = listenerIds = listenerIds + 1;
            this.listener.Add(nextId, listener);
            //Logging.Log("Event Add End     : " + uniqueId);
            return nextId;
        }

        public virtual void Remove(uint id)
        {
            //Logging.Log("Event Remove      : " + uniqueId);
            this.listener.Remove(id);
            //Logging.Log("Event Remove End  : " + uniqueId);
        }

        public void Trigger(T val)
        {


            //Logging.Log("Event Trigger     : " + uniqueId);

            List<Action<T>> actions = new List<Action<T>>(this.listener.Values);
            foreach ( Action<T> action in actions )
            {
                action(val);
            }

            //List<uint> keys = new List<uint>(this.listener.Keys);

            //Action<T> action;

            //foreach (uint id in keys)
            //{
            //    if ( this.listener.TryGetValue(id, out action) )
            //    {
            //        action(val);
            //    }
            //}

            //Logging.Log("Event Trigger End : " + uniqueId);
        }

        public uint UniqueId
        {
            get { return uniqueId; }
            private set { }
        }
    }

    public class EventObservable<T> : Event<T>
    {
        public Event<int> eventAdd = new Event<int>();
        public Event<int> eventRemove = new Event<int>();

        public override uint Add(Action<T> listener)
        {
            uint nextId = base.Add(listener);
            eventAdd.Trigger(this.listener.Count);
            return nextId;
        }

        public override void Remove(uint id)
        {
            base.Remove(id);
            eventRemove.Trigger(this.listener.Count);
        }
    }
}
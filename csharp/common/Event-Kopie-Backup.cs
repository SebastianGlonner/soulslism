using System;
using System.Collections.Generic;

namespace Soulslism
{
    public class EventsMemory<T>
    {

        private struct EventingPair
        {
            public int attachedId;
            public Event<T> eventing;
        }

        private System.Collections.Generic.Dictionary<uint, EventingPair> attachedEventings = new System.Collections.Generic.Dictionary<uint, EventingPair>();

        protected void AttachToEventing(Event<T> eventing, Action<T> method)
        {
            uint eventingId = eventing.UniqueId;
            if (attachedEventings.ContainsKey(eventingId))
            {
                return;
            }

            EventingPair eventingPair = new EventingPair();
            eventingPair.attachedId = eventing.Add(method);
            eventingPair.eventing = eventing;

            attachedEventings.Add(eventingId, eventingPair);
        }

        protected void DetachFromEventing(Event<T> eventing)
        {
            uint eventingId = eventing.UniqueId;
            EventingPair eventingPair;

            if (attachedEventings.TryGetValue(eventingId, out eventingPair))
            {
                eventing.Remove(eventingPair.attachedId);
                attachedEventings.Remove(eventingId);
            }
        }

        public void OnQueueFree()
        {
            foreach (KeyValuePair<uint, EventingPair> kvp in attachedEventings)
            {
                EventingPair eventingPair = kvp.Value;
                eventingPair.eventing.Remove(eventingPair.attachedId);
            }

            attachedEventings = null;
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

        public interface IEventingListener
        {
            void OnNamedEvent(int identifier);
        }

        protected int listenerIds = 0;

        protected Dictionary<int, Action<T>> listener;

        private readonly uint uniqueId;

        public Event()
        {
            uniqueId = Event<T>.GetUniqueId();
        }

        public int Add(Action<T> listener)
        {
            int nextId = listenerIds = listenerIds + 1;
            this.listener.Add(nextId, listener);
            return nextId;
        }

        public void Remove(int id)
        {
            this.listener.Remove(id);
        }

        public void Trigger(T val)
        {
            foreach (KeyValuePair<int, Action<T>> kvp in this.listener)
            {
                kvp.Value(val);
            }
        }

        public uint UniqueId
        {
            get { return uniqueId; }
            private set { }
        }
    }
}
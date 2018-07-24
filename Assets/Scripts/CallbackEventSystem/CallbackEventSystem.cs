using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks
{
    public class CallbackEventSystem : MonoBehaviour
    {

        // Use this for initialization
        void Start()
        {

        }

        static private CallbackEventSystem __Current;
        static public CallbackEventSystem Current
        {
            get
            {
                if (__Current == null)
                    __Current = GameObject.FindObjectOfType<CallbackEventSystem>();

                return __Current;
            }
        }

        public delegate void EventListener(EventInfo ei);
        public enum EVENT_TYPE { CHARACTER_SPAWN, CHARACTER_ATTACK, CHARACTER_DIED }
        Dictionary<EVENT_TYPE, List<EventListener>> eventListeners;

        public void RegisterListener(EVENT_TYPE eventType, EventListener listener)
        {
            if (eventListeners == null)
                eventListeners = new Dictionary<EVENT_TYPE, List<EventListener>>();

            if (!eventListeners.ContainsKey(eventType) || eventListeners[eventType] == null)
                eventListeners[eventType] = new List<EventListener>();

            eventListeners[eventType].Add(listener);
        }

        public void UnregisterListener(EVENT_TYPE everntTYpe, EventListener listener)
        {
            // TODO...
        }

        public void FireEvent(EVENT_TYPE eventType, EventInfo eventInfo)
        {
            if (eventListeners == null || eventListeners[eventType] == null)
            {
                // No onme is listening, we're done here.
                return;
            }

            foreach (EventListener el in eventListeners[eventType])
            {
                el(eventInfo);
            }
        }

    }
}

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks
{
    // Generic where <T> must be a 'decendent' of Event
    public abstract class EventInfo<T> where T : EventInfo<T>
    {
        // Base Event class
        public string EventDescription;

        public delegate void EventListener(T info);
        // Static for generic, means _Listeners is NOT shared between all instances of EventInfo
        // Rather a separate instance is shared between each set DIFFERENT decendent of EventInfo
        // eg. All DeathEventInfo's share the same _Listeners, but all DebugEventInfo share a different _Listeners
        private static event EventListener _Listeners;

        public static void RegisterListener(EventListener listener)
        {
            _Listeners += listener;
        }
        public static void UnregisterListener(EventListener listener)
        {
            _Listeners -= listener;
        }

        // 'T' ensures the correct Type of eventInfo is used for the relevant listener
        public void FireEvent()
        {
            if (_Listeners != null)
                _Listeners(this as T);
        }

    }
    public class BattleWonEventInfo : EventInfo<BattleWonEventInfo>
    {
        // info about what has been earned
        // bounty, xp, items... etc
    }

    // TODO.. look into making use of this
    public class DebugEventInfo : EventInfo<DebugEventInfo>
    {
        public int SeverityLevel;
    }    

    public class DeathEventInfo : EventInfo<DeathEventInfo>
    {
        // Info about cause of death, killer, etc
        public GameObject UnitGO;
        public TeamName TeamName;
    }

    public class HeroDeathEventInfo : EventInfo<HeroDeathEventInfo>
    {
        // Battle is lost if this happens
        // Battle is all there is atm so information required here will 
        // be filled out later
    }

    public class TakeDamageEventInfo : EventInfo<TakeDamageEventInfo>
    {
        // Info about the attack the character has just been the target of.
        public int Damage;
        public GameObject UnitGO;
        // more info can go here
        // eg. Who attacked
    }

    public class UnitSpawnEventInfo : EventInfo<UnitSpawnEventInfo>
    {
        public GameObject UnitGO;
        public GameObject UnitSlotGO;
    }
}
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace EventCallbacks
{
    public abstract class EventInfo
    {
        // Base Event info
        public string EventDescription;

    }

    // TODO.. look into making use of this
    public class DebugEventInfo
    {
        public int SeverityLevel;
    }

    public class DeathEventInfo : EventInfo
    {
        // Info about cause of death, killer, etc
        public GameObject UnitGO;
    }
}
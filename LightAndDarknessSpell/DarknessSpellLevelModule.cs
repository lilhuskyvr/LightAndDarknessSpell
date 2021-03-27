using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Local

namespace LightAndDarknessSpell
{
    public class DarknessSpellLevelModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine(Level level)
        {
            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;

            return base.OnLoadCoroutine(level);
        }

        private void EventManagerOnonCreatureSpawn(Creature creature)
        {
            if (creature.data.id.Contains("Shadow"))
            {
                if (creature.factionId != 2)
                {
                    creature.Hide(true);
                    var shadow = creature.gameObject.AddComponent<Shadow>();
                    shadow.Init(false, true);
                }
            }
        }
    }
}
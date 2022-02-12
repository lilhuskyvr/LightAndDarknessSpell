using System.Collections;
using ThunderRoad;

// ReSharper disable UnusedMember.Local

namespace LightAndDarknessSpell
{
    public class DarknessSpellLevelModule : LevelModule
    {
        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;

            return base.OnLoadCoroutine();
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
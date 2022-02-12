using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Local

namespace LightAndDarknessSpell
{
    public class LightAndDarknessSpellLevelModule : LevelModule
    {
        private LightAndDarknessSpellController _lightAndDarknessSpellController;

        public override IEnumerator OnLoadCoroutine()
        {
            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;
            EventManager.onLevelLoad += EventManagerOnonLevelLoad;
            _lightAndDarknessSpellController =
                GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();

            return base.OnLoadCoroutine();
        }

        private void EventManagerOnonLevelLoad(LevelData leveldata, EventTime eventtime)
        {
            if (_lightAndDarknessSpellController != null)
                _lightAndDarknessSpellController.darknessSpellController.isTimeStopped = false;
        }

        private void EventManagerOnonCreatureSpawn(Creature creature)
        {
            if (!creature.isPlayer)
            {
                try
                {
                    Object.Destroy(creature.gameObject.GetComponent<FrozenRagdollCreature>());
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }

                try
                {
                    Object.Destroy(creature.gameObject.GetComponent<FrozenAnimationCreature>());
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }
        }

        public override void Update()
        {
            if (_lightAndDarknessSpellController == null)
            {
                _lightAndDarknessSpellController =
                    GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();
                return;
            }

            if (!_lightAndDarknessSpellController.darknessSpellController.isTimeStopped)
                return;
            foreach (var creature in Creature.all)
            {
                if (!creature.isPlayer)
                {
                    if (creature.gameObject.GetComponent<FrozenRagdollCreature>() == null)
                        creature.gameObject.AddComponent<FrozenRagdollCreature>();
                    if (creature.gameObject.GetComponent<FrozenAnimationCreature>() == null)
                        creature.gameObject.AddComponent<FrozenAnimationCreature>();
                }
            }

            foreach (var item in Item.all)
            {
                if (item.isThrowed || item.isFlying)
                {
                    if (item.gameObject.GetComponent<FrozenItem>() == null)
                        item.gameObject.AddComponent<FrozenItem>();
                }
            }
        }
    }
}
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using ThunderRoad;
using Debug = UnityEngine.Debug;
using Object = UnityEngine.Object;

// ReSharper disable UnusedMember.Local

namespace LightAndDarknessSpell
{
    public class LightSpellLevelModule : LevelModule
    {
        private List<Creature> _creatures;

        public override IEnumerator OnLoadCoroutine(Level level)
        {
            EventManager.onCreatureHit += EventManagerOnonCreatureHit;
            EventManager.onCreatureParry += EventManagerOnonCreatureParry;

            EventManager.onCreatureSpawn += EventManagerOnonCreatureSpawn;

            _creatures = new List<Creature>();

            return base.OnLoadCoroutine(level);
        }

        private void EventManagerOnonCreatureSpawn(Creature creature)
        {
            if (creature.name.Contains("Angel"))
            {
                if (creature.factionId != 2)
                {
                    Debug.Log("Enemy Angel");
                    //enemy angel
                    var angel = creature.gameObject.AddComponent<Angel>();
                    angel.Init(false, true);
                }
            }
        }

        private void EventManagerOnonCreatureParry(Creature creature, CollisionInstance collisioninstance)
        {
            try
            {
                var sourceCreature = collisioninstance.sourceColliderGroup.collisionHandler.item.mainHandler.creature;

                if (sourceCreature.gameObject.GetComponent<Angel>() != null)
                {
                    collisioninstance.targetColliderGroup.collisionHandler.item.mainHandler.TryRelease();
                    collisioninstance.targetColliderGroup.collisionHandler.item.rb.AddForce(collisioninstance
                        .impactVelocity);
                    collisioninstance.targetColliderGroup.collisionHandler.item.mainHandler.creature.brain.instance
                        .TryKnockout(collisioninstance);
                }
            }
            catch (Exception exception)
            {
                //ignored
            }
        }

        private IEnumerator BanishCreature(Creature creature)
        {
            if (!_creatures.Contains(creature))
            {
                _creatures.Add(creature);
                var ladc = GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>();

                GameManager.local.StartCoroutine(
                    ladc.lightSpellController.HeavenSwordOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Head),
                        0));
                GameManager.local.StartCoroutine(
                    ladc.lightSpellController.HeavenSwordOnRagdollPart(
                        creature.ragdoll.GetPart(RagdollPart.Type.LeftArm), 0));
                yield return GameManager.local.StartCoroutine(
                    ladc.lightSpellController.HeavenSwordOnRagdollPart(
                        creature.ragdoll.GetPart(RagdollPart.Type.RightArm), 0));
                _creatures.Remove(creature);
            }

            yield return null;
        }

        private void EventManagerOnonCreatureHit(Creature creature, CollisionInstance collisioninstance)
        {
            try
            {
                var sourceCreature = collisioninstance.sourceColliderGroup.collisionHandler.item.mainHandler.creature;

                if (sourceCreature.gameObject.GetComponent<Angel>() != null)
                {
                    if (!creature.isPlayer && creature.state != Creature.State.Dead && creature.factionId != 2)
                    {
                        GameManager.local.StartCoroutine(BanishCreature(creature));
                    }
                }
            }
            catch (Exception exception)
            {
                //ignored
            }
        }
    }
}
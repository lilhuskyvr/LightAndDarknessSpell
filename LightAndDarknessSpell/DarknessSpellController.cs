using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LightAndDarknessSpell
{
    public class DarknessSpellController
    {
        public bool isTimeStopped;
        private float _saturationMultiplier = 1.0f;
        private bool _isShadowWalking;
        private EffectData _spawnEffectData;
        private EffectData _blinkEffectData;

        public DarknessSpellController()
        {
            _spawnEffectData = Catalog.GetData<EffectData>("SpawnShadow");
            _blinkEffectData = Catalog.GetData<EffectData>("ShadowWalk");
        }

        private IEnumerator DrainingBlood(Creature creature)
        {
            while (creature.state != Creature.State.Dead)
            {
                creature.TestDamage();
                yield return new WaitForSeconds(1);
            }

            yield return null;
        }

        public void TryDrainingBlood(CollisionInstance collisionInstance)
        {
            try
            {
                //try draining blood a creature
                var targetCreature =
                    collisionInstance.targetCollider.attachedRigidbody.GetComponentInParent<Creature>();

                GameManager.local.StartCoroutine(DrainingBlood(targetCreature));
            }
            catch (Exception exception)
            {
                //ignore
            }
        }

        public void SpawnShadow(Vector3 position)
        {
            var random = new System.Random();

            var creatureId = random.Next(1, 101) <= GameManager.options.maleRatio ? "ShadowMale" : "ShadowFemale";
            var creature = Catalog.GetData<CreatureData>(creatureId);

            var rotation = Player.local.transform.rotation;
            var spawnEffect = _spawnEffectData.Spawn(position + Vector3.up, rotation);
            spawnEffect.Play();
            GameManager.local.StartCoroutine(creature.SpawnCoroutine(position, rotation, null,
                rsCreature =>
                {
                    rsCreature.Hide(true);
                    rsCreature.gameObject.AddComponent<Banshee>();
                }));
        }

        private IEnumerator StopTime()
        {
            isTimeStopped = true;

            PostProcessManager.SetSepia(_saturationMultiplier);
            yield return null;
        }

        private IEnumerator ResumeTime()
        {
            foreach (var creature in Creature.list)
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

            foreach (var item in Item.list)
            {
                try
                {
                    Object.Destroy(item.gameObject.GetComponent<FrozenItem>());
                }
                catch (Exception e)
                {
                    Debug.Log(e.Message);
                }
            }

            isTimeStopped = false;

            PostProcessManager.RefreshPostProcess();

            yield return null;
        }

        private static IEnumerator BlinkToPosition(Player player, Vector3 position, Vector3 forward)
        {
            var items = new List<Item>();

            player.locomotion.rb.isKinematic = true;
            player.locomotion.isGrounded = false;

            foreach (var side in new[] {Side.Left, Side.Right})
            {
                try
                {
                    player.creature.GetHand(side).grabbedHandle.item.rb.detectCollisions = false;
                    items.Add(player.creature.GetHand(side).grabbedHandle.item);
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                }
            }


            player.transform.position = position;
            player.transform.rotation *= Quaternion.FromToRotation(player.creature.transform.forward, forward);
            player.locomotion.rb.isKinematic = false;

            yield return new WaitForSeconds(0.5f);

            foreach (var item in items)
            {
                item.rb.detectCollisions = true;
            }

            yield return null;
        }

        private IEnumerator BlinkBehindNpcCoroutine(Player player, Creature npc)
        {
            var direction = (npc.transform.position - player.transform.position).normalized.ToXZ();
            var items = new List<Item>();

            player.locomotion.rb.isKinematic = true;
            player.locomotion.isGrounded = false;

            foreach (var side in new[] {Side.Left, Side.Right})
            {
                try
                {
                    player.creature.GetHand(side).grabbedHandle.item.rb.detectCollisions = false;
                    items.Add(player.creature.GetHand(side).grabbedHandle.item);
                }
                catch (Exception exception)
                {
                    Debug.Log(exception.Message);
                }
            }

            var effectPosition = player.transform.position;

            var blinkEffect = _blinkEffectData.Spawn(effectPosition, Quaternion.identity);
            blinkEffect.Play();

            player.transform.position = npc.transform.position + direction;
            player.transform.rotation *=
                Quaternion.FromToRotation(player.creature.transform.forward,
                    npc.transform.position - player.transform.position);
            player.locomotion.rb.isKinematic = false;

            yield return new WaitForSeconds(0.5f);

            foreach (var item in items)
            {
                item.rb.detectCollisions = true;
            }

            yield return null;
        }

        public IEnumerator ShadowWalkCoroutine()
        {
            if (!_isShadowWalking)
            {
                _isShadowWalking = true;
                yield return StopTime();

                var startPosition = Player.local.creature.transform.position;
                var startForward = Player.local.creature.transform.forward;

                var creatures = new List<Creature>(Creature.list);

                foreach (var creature in creatures)
                {
                    if (!creature.isPlayer && creature.state == Creature.State.Alive && creature.factionId != 2)
                    {
                        yield return BlinkBehindNpcCoroutine(Player.local, creature);
                        yield return new WaitForSeconds(2);
                    }
                }

                yield return BlinkToPosition(Player.local, startPosition, startForward);

                yield return ResumeTime();
                _isShadowWalking = false;
            }

            yield return null;
        }

        public void ShadowWalk()
        {
            GameManager.local.StartCoroutine(ShadowWalkCoroutine());
        }
    }
}
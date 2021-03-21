using System;
using System.Collections;
using System.Collections.Generic;
using ThunderRoad;
using UnityEngine;
using Random = System.Random;

namespace LightAndDarknessSpell
{
    public class LightSpellController
    {
        private List<ItemData> _swordItemDatas;
        private Random _random;

        public LightSpellController()
        {
            _random = new Random();
            _swordItemDatas = new List<ItemData>();
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordShortCommonHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordShortAnticHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordLongReverendLostHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordLongReverendHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordLongCommonHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordGreatClaymoreHeaven"));
        }

        private IEnumerator ExplodeCreature(Creature creature, Vector3 dir)
        {
            var random = new System.Random();
            while (Time.time - creature.spawnTime <= 2)
            {
                yield return new WaitForFixedUpdate();
            }

            string[] parts =
            {
                "LeftHand", "LeftForeArm", "LeftArm",
                "RightHand", "RightForeArm", "RightArm",
                "LeftFoot", "LeftLeg",
                "RightFoot", "RightLeg",
                "Head"
            };

            int index = 0;

            while (index < parts.Length)
            {
                foreach (var part in creature.ragdoll.parts)
                {
                    try
                    {
                        part.rb.AddForce((dir + Vector3.up),
                            ForceMode.Impulse);

                        if (part.name == parts[index])
                        {
                            if (random.Next(1, 101) <= 50)
                                part.Slice();
                        }
                    }
                    catch (Exception exception)
                    {
                        Debug.Log(exception.Message);
                    }
                }

                index++;
            }

            if (creature.state != Creature.State.Dead)
                creature.Kill();

            yield return null;
        }

        public void TryBanish(CollisionInstance collisionInstance)
        {
            try
            {
                //try banish a creature
                var targetCreature =
                    collisionInstance.targetCollider.attachedRigidbody.GetComponentInParent<Creature>();

                GameManager.local.StartCoroutine(ExplodeCreature(targetCreature,
                    collisionInstance.impactVelocity));
            }
            catch (Exception exception)
            {
                //ignore
            }
        }

        public void SpawnAngel(Vector3 position)
        {
            var random = new System.Random();

            var creatureId = random.Next(1, 101) <= GameManager.options.maleRatio ? "HumanMale" : "HumanFemale";
            var creature = Catalog.GetData<CreatureData>(creatureId);

            creature.containerID = "Knight1HShield";
            creature.brainId = "BaseWarrior";

            var rotation = Player.local.transform.rotation;
            GameManager.local.StartCoroutine(creature.SpawnCoroutine(position, rotation, null,
                rsCreature =>
                {
                    rsCreature.SetFaction(2);
                    rsCreature.gameObject.AddComponent<Angel>();
                }));
        }

        public IEnumerator FallingSword(Item swordItem, RagdollPart ragdollPart)
        {
            var startTime = Time.time;
            swordItem.SetColliderAndMeshLayer(GameManager.GetLayer(LayerName.MovingObject));
            swordItem.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            swordItem.isThrowed = true;
            swordItem.isFlying = true;
            swordItem.rb.useGravity = false;
            while (!swordItem.isPenetrating)
            {
                if (Time.time - startTime <= 30)
                {
                    var position = ragdollPart.transform.position;
                    var direction = (position - swordItem.transform.position).normalized;
                    swordItem.rb.velocity = 30 * direction;
                    swordItem.transform.rotation *= Quaternion.FromToRotation(swordItem.flyDirRef.forward, direction);

                    yield return new WaitForFixedUpdate();
                }
            }

            swordItem.Despawn(10);

            yield return null;
        }

        public IEnumerator HeavenSwordOnRagdollPart(RagdollPart ragdollPart, float timeout)
        {
            var startTime = Time.time;
            while (Time.time - startTime <= timeout)
            {
                yield return new WaitForSeconds(1);
            }

            var heaven = ragdollPart.transform.position + 5 * Vector3.up;
            var swordItemData = _swordItemDatas[_random.Next(0, _swordItemDatas.Count)];
            swordItemData.SpawnAsync(swordItem =>
            {
                foreach (var renderer in swordItem.renderers)
                {
                    foreach (var material in renderer.materials)
                    {
                        if (material.HasProperty("_BaseColor"))
                            material.SetColor("_BaseColor", new Color(25, 25, 25, 1));
                    }
                }

                GameManager.local.StartCoroutine(FallingSword(swordItem, ragdollPart));
            }, heaven, Quaternion.identity);

            yield return null;
        }

        public IEnumerator HeavenlySwordsCoroutine()
        {

            foreach (var creature in Creature.list)
            {
                if (!creature.isPlayer && creature.state != Creature.State.Dead && creature.factionId != 2)
                {
                    GameManager.local.StartCoroutine(
                        HeavenSwordOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Head), 0));
                    GameManager.local.StartCoroutine(
                        HeavenSwordOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftArm), 1));
                    GameManager.local.StartCoroutine(
                        HeavenSwordOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightArm), 2));
                    GameManager.local.StartCoroutine(
                        HeavenSwordOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Torso), 3));
                    GameManager.local.StartCoroutine(
                        HeavenSwordOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftLeg), 4));
                    GameManager.local.StartCoroutine(
                        HeavenSwordOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightLeg), 5));
                }
            }

            yield return null;
        }

        public void HeavenlySwords()
        {
            GameManager.local.StartCoroutine(HeavenlySwordsCoroutine());
        }
    }
}
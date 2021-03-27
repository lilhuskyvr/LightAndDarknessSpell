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
        private List<ItemData> _objectItemDatas;
        private ItemData _fireArrowItemData;
        private ItemData _lightningItemData;
        private Random _random;
        private EffectData _spawnEffectData;
        public Color angelColor;
        private List<Creature> _creatures;

        public LightSpellController()
        {
            _random = new Random();
            _swordItemDatas = new List<ItemData>();
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordShortCommonHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordShortAnticHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordLongReverendHeaven"));
            _swordItemDatas.Add(Catalog.GetData<ItemData>("SwordLongCommonHeaven"));

            _objectItemDatas = new List<ItemData>();

            for (int i = 0; i < Catalog.data.Length; i++)
            {
                foreach (CatalogData catalogData in Catalog.data[i])
                {
                    if (catalogData is ItemData)
                    {
                        ItemData itemData = catalogData as ItemData;
                        if (itemData.mass >= 50 && !itemData.id.Contains("DeadTreeSpikes"))
                            _objectItemDatas.Add(itemData);
                    }
                }
            }

            _fireArrowItemData = Catalog.GetData<ItemData>("Arrow");
            _lightningItemData = Catalog.GetData<ItemData>("DaggerCommon");
            _spawnEffectData = Catalog.GetData<EffectData>("SpawnAngel");

            angelColor = new Color(25, 25, 25, 1);
            _creatures = new List<Creature>();
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
            var random = new Random();

            var creatureId = random.Next(1, 101) <= GameManager.options.maleRatio ? "AngelMale" : "AngelFemale";
            var creatureData = Catalog.GetData<CreatureData>(creatureId);
            var rotation = Player.local.transform.rotation;

            var spawnEffect = _spawnEffectData.Spawn(position + Vector3.up, rotation);
            spawnEffect.Play();

            GameManager.local.StartCoroutine(creatureData.SpawnCoroutine(position, rotation, null,
                rsCreature =>
                {
                    rsCreature.Hide(true);
                    var angel = rsCreature.gameObject.AddComponent<Angel>();
                    angel.Init(true, false, GameManager.local.gameObject.GetComponent<LightAndDarknessSpellController>()
                        .lightSpellController.angelColor);
                }));
        }

        public IEnumerator FallingItem(Item item, RagdollPart ragdollPart)
        {
            var startTime = Time.time;
            item.SetColliderAndMeshLayer(GameManager.GetLayer(LayerName.MovingObject));
            item.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            item.isThrowed = true;
            item.isFlying = true;
            while (item.isFlying)
            {
                if (Time.time - startTime <= 30)
                {
                    var position = ragdollPart.transform.position;
                    var direction = (position - item.transform.position).normalized;
                    item.rb.velocity = 30 * direction;
                    item.transform.rotation *= Quaternion.FromToRotation(item.flyDirRef.forward, direction);

                    yield return new WaitForFixedUpdate();
                }
            }

            item.Despawn(10);

            yield return null;
        }

        public IEnumerator FallingObjectItem(Item objectItem, RagdollPart ragdollPart)
        {
            var startTime = Time.time;
            objectItem.SetColliderAndMeshLayer(GameManager.GetLayer(LayerName.MovingObject));
            objectItem.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            objectItem.isThrowed = true;
            objectItem.isFlying = true;

            var position = ragdollPart.transform.position;
            var direction = (position - objectItem.transform.position).normalized;
            objectItem.rb.velocity = 100 * direction;

            objectItem.Despawn(10);

            yield return null;
        }

        public IEnumerator FallingFireArrows(Item fireArrowItem, RagdollPart ragdollPart)
        {
            fireArrowItem.SetColliderAndMeshLayer(GameManager.GetLayer(LayerName.MovingObject));
            fireArrowItem.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            fireArrowItem.isThrowed = true;
            fireArrowItem.isFlying = true;

            while (fireArrowItem.imbues.Count == 0)
            {
                yield return new WaitForFixedUpdate();
            }

            fireArrowItem.imbues[0]
                .Transfer(Catalog.GetData<SpellCastCharge>("Fire"), fireArrowItem.imbues[0].maxEnergy);

            var startTime = Time.time;
            while (!fireArrowItem.isPenetrating)
            {
                if (Time.time - startTime <= 30)
                {
                    var position = ragdollPart.transform.position;
                    var direction = (position - fireArrowItem.transform.position).normalized;
                    fireArrowItem.rb.velocity = 30 * direction;
                    fireArrowItem.transform.rotation *=
                        Quaternion.FromToRotation(fireArrowItem.flyDirRef.forward, direction);

                    yield return new WaitForFixedUpdate();
                }
            }

            fireArrowItem.Despawn(10);

            yield return null;
        }

        public IEnumerator FallingLightning(Item lightningItem, RagdollPart ragdollPart)
        {
            lightningItem.Hide(true);
            lightningItem.SetColliderAndMeshLayer(GameManager.GetLayer(LayerName.MovingObject));
            lightningItem.rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            lightningItem.isThrowed = true;
            lightningItem.isFlying = true;

            while (lightningItem.imbues.Count == 0)
            {
                yield return new WaitForFixedUpdate();
            }

            lightningItem.imbues[0].Transfer(Catalog.GetData<SpellCastCharge>("Lightning"),
                lightningItem.imbues[0].maxEnergy);

            var startTime = Time.time;
            while (!lightningItem.isPenetrating)
            {
                if (Time.time - startTime <= 30)
                {
                    var position = ragdollPart.transform.position;
                    var direction = (position - lightningItem.transform.position).normalized;
                    lightningItem.rb.velocity = 30 * direction;
                    lightningItem.transform.rotation *=
                        Quaternion.FromToRotation(lightningItem.flyDirRef.forward, direction);

                    yield return new WaitForFixedUpdate();
                }
            }

            lightningItem.Despawn(10);

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
                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (material.HasProperty("_BaseColor"))
                            material.SetColor("_BaseColor", angelColor);
                    }
                }

                GameManager.local.StartCoroutine(FallingItem(swordItem, ragdollPart));
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

        public IEnumerator HeavenFireArrowOnRagdollPart(RagdollPart ragdollPart, float timeout)
        {
            var startTime = Time.time;
            while (Time.time - startTime <= timeout)
            {
                yield return new WaitForSeconds(1);
            }

            var heaven = ragdollPart.transform.position + 5 * Vector3.up;
            _fireArrowItemData.SpawnAsync(
                meteorItem =>
                {
                    meteorItem.transform.position = heaven;
                    GameManager.local.StartCoroutine(FallingFireArrows(meteorItem, ragdollPart));
                }, heaven,
                Quaternion.identity);

            yield return null;
        }

        public IEnumerator HeavenlyPunishOnRagdollPart(RagdollPart ragdollPart, ItemData itemData, float timeout)
        {
            var startTime = Time.time;
            while (Time.time - startTime <= timeout)
            {
                yield return new WaitForSeconds(1);
            }

            var heaven = ragdollPart.transform.position + 5 * Vector3.up;
            itemData.SpawnAsync(
                item =>
                {
                    item.transform.position = heaven;
                    PaintItem(item);
                    GameManager.local.StartCoroutine(FallingItem(item, ragdollPart));
                }, heaven,
                Quaternion.identity);

            yield return null;
        }

        public IEnumerator HeavenLightningOnRagdollPart(RagdollPart ragdollPart, float timeout)
        {
            var startTime = Time.time;
            while (Time.time - startTime <= timeout)
            {
                yield return new WaitForSeconds(1);
            }

            var heaven = ragdollPart.transform.position + 5 * Vector3.up;
            _lightningItemData.SpawnAsync(
                lightningItem =>
                {
                    lightningItem.transform.position = heaven;
                    GameManager.local.StartCoroutine(FallingLightning(lightningItem, ragdollPart));
                }, heaven,
                Quaternion.identity);

            yield return null;
        }

        public IEnumerator HeavenlyObjectOnRagdollPart(RagdollPart ragdollPart, float timeout)
        {
            var startTime = Time.time;
            while (Time.time - startTime <= timeout)
            {
                yield return new WaitForSeconds(1);
            }

            var heaven = ragdollPart.transform.position + 5 * Vector3.up;
            var objectItemData = _objectItemDatas[_random.Next(0, _objectItemDatas.Count)];
            objectItemData.SpawnAsync(
                objectItem => { GameManager.local.StartCoroutine(FallingObjectItem(objectItem, ragdollPart)); }, heaven,
                Quaternion.identity);

            yield return null;
        }

        public IEnumerator HeavenlyFireArrowsCoroutine()
        {
            foreach (var creature in Creature.list)
            {
                if (!creature.isPlayer && creature.state != Creature.State.Dead && creature.factionId != 2)
                {
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Head), 0));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftArm), 0));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightArm), 0));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Torso), 0.5f));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftLeg), 0.5f));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightLeg), 0.5f));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftHand), 1));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightHand), 1));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftFoot), 1));
                    GameManager.local.StartCoroutine(
                        HeavenFireArrowOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightFoot), 1));
                }
            }

            yield return null;
        }

        public IEnumerator HeavenlyLightningCoroutine()
        {
            foreach (var creature in Creature.list)
            {
                if (!creature.isPlayer && creature.state != Creature.State.Dead && creature.factionId != 2)
                {
                    GameManager.local.StartCoroutine(
                        HeavenLightningOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Head), 0));
                    GameManager.local.StartCoroutine(
                        HeavenLightningOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftArm), 0));
                    GameManager.local.StartCoroutine(
                        HeavenLightningOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightArm), 0));
                    GameManager.local.StartCoroutine(
                        HeavenLightningOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Torso), 0));
                }
            }

            yield return null;
        }

        public IEnumerator HeavenlyObjectsCoroutine()
        {
            foreach (var creature in Creature.list)
            {
                if (!creature.isPlayer && creature.state != Creature.State.Dead && creature.factionId != 2)
                {
                    GameManager.local.StartCoroutine(
                        HeavenlyObjectOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Head), 0));
                }
            }

            yield return null;
        }

        public void HeavenlyFireArrows()
        {
            GameManager.local.StartCoroutine(HeavenlyFireArrowsCoroutine());
        }

        public void HeavenlyLightning()
        {
            GameManager.local.StartCoroutine(HeavenlyLightningCoroutine());
        }

        public void HeavenlyObjects()
        {
            GameManager.local.StartCoroutine(HeavenlyObjectsCoroutine());
        }

        public void PaintItem(Item item)
        {
            foreach (var renderer in item.renderers)
            {
                foreach (var material in renderer.materials)
                {
                    if (material.HasProperty("_BaseColor"))
                    {
                        material.SetColor("_BaseColor", angelColor);
                    }
                }
            }
        }

        public IEnumerator HeavenlyPunishCoroutine(Creature creature, Item item)
        {
            if (!_creatures.Contains(creature))
            {
                _creatures.Add(creature);
                GameManager.local.StartCoroutine(
                    HeavenlyPunishOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Head), item.data, 0));
                GameManager.local.StartCoroutine(
                    HeavenlyPunishOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftArm), item.data, 0));
                GameManager.local.StartCoroutine(
                    HeavenlyPunishOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightArm), item.data, 0));
                GameManager.local.StartCoroutine(
                    HeavenlyPunishOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.Torso), item.data, 0.5f));
                GameManager.local.StartCoroutine(
                    HeavenlyPunishOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.LeftLeg), item.data, 0.5f));
                yield return GameManager.local.StartCoroutine(
                    HeavenlyPunishOnRagdollPart(creature.ragdoll.GetPart(RagdollPart.Type.RightLeg), item.data, 0.5f));
                _creatures.Remove(creature);
            }

            yield return null;
        }

        public void HeavenlyPunish(Creature creature, Item item)
        {
            GameManager.local.StartCoroutine(HeavenlyPunishCoroutine(creature, item));
        }
    }
}
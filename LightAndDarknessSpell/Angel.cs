using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;
using Random = System.Random;

namespace LightAndDarknessSpell
{
    public class Angel : MonoBehaviour
    {
        private Creature _creature;
        private Random _random;
        private bool _isExploding;
        public bool isSelfDestroy;
        public bool hasRagdoll;
        public Color angelColor;

        private void Awake()
        {
            _creature = GetComponent<Creature>();
            _random = new Random();
        }

        public void Init(bool isSelfDestroyParam, bool hasRagdollParam, Color angelColorParam)
        {
            isSelfDestroy = isSelfDestroyParam;
            hasRagdoll = hasRagdollParam;
            angelColor = angelColorParam;
            StartCoroutine(AngelTransformation());
        }

        private void Update()
        {
            if (_creature == null || !isSelfDestroy)
            {
                return;
            }

            if (Time.time - _creature.spawnTime >= 30)
                _creature.Kill();
        }

        public void Banish(Creature creature, Vector3 damageDirection)
        {
            if (creature.factionId == _creature.factionId || creature.isPlayer) return;
            var rand = _random.Next(1, 101);
            //20% banish
            if (rand <= 20)
                StartCoroutine(ExplodeCreature(creature, damageDirection));
        }

        private IEnumerator ExplodeCreature(Creature creature, Vector3 dir)
        {
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
                            if (_random.Next(1, 101) <= 50)
                            {
                                part.Slice();
                            }
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

        private IEnumerator AngelExplosionCoroutine()
        {
            if (!_isExploding)
            {
                _isExploding = true;
                var explosionCenter = _creature.transform.position + 0.5f * _creature.morphology.height * Vector3.up;

                var colliders = Physics.OverlapSphere(explosionCenter, 5);

                foreach (var collider in colliders)
                {
                    if (collider.attachedRigidbody != null)
                    {
                        var dir = (collider.transform.position - explosionCenter).normalized;

                        var creature = collider.attachedRigidbody.GetComponentInParent<Creature>();

                        if (creature != null)
                        {
                            if (creature.factionId != _creature.factionId)
                                StartCoroutine(ExplodeCreature(creature, dir));
                        }
                        else
                        {
                            var item = collider.attachedRigidbody.GetComponentInParent<Item>();

                            if (item != null)
                            {
                                if (!item.isGripped)
                                {
                                    item.rb.AddForce(2 * (dir + Vector3.up), ForceMode.Impulse);
                                }
                            }
                        }
                    }
                }

                _isExploding = false;
            }

            Destroy(this);
            yield return null;
        }

        private IEnumerator AngelTransformation()
        {
            while (Time.time - _creature.spawnTime <= 1)
            {
                yield return new WaitForFixedUpdate();
            }
            
            _creature.equipment.EquipAllWardrobes(false);

            _creature.ragdoll.enabled = hasRagdoll;
            foreach (var part in _creature.manikinLocations.PartList.GetAllParts())
            {
                foreach (var renderer in part.GetRenderers())
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (material.HasProperty("_BaseColor"))
                        {
                            material.SetColor("_BaseColor", angelColor);
                        }
                    }
                }
            }

            foreach (var holder in _creature.equipment.holders)
            {
                foreach (var item in holder.items)
                {
                    foreach (var renderer in item.renderers)
                    {
                        foreach (var material in renderer.sharedMaterials)
                        {
                            if (material.HasProperty("_BaseColor"))
                            {
                                material.SetColor("_BaseColor", angelColor);
                            }
                        }
                    }
                }
            }


            foreach (var side in new[] {Side.Left, Side.Right})
            {
                var weapon = _creature.equipment.GetHeldobject(side);

                if (weapon != null)
                {
                    foreach (var renderer in weapon.renderers)
                    {
                        foreach (var material in renderer.sharedMaterials)
                        {
                            if (material.HasProperty("_BaseColor"))
                            {
                                material.SetColor("_BaseColor", angelColor);
                            }
                        }
                    }
                }
            }

            _creature.OnKillEvent += (instance, time) =>
            {
                if (time == EventTime.OnEnd)
                {
                    Destroy(this);
                }
            };

            _creature.Hide(false);
            yield return null;
        }
    }
}
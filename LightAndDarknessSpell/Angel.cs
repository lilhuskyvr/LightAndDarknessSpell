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

        private void Start()
        {
            _creature = GetComponent<Creature>();
            _random = new Random();
            StartCoroutine(AngelTransformation());
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

            var brain = _creature.brain.instance as BrainHuman;
            brain.canLeave = false;
            brain.allowDisarm = false;

            foreach (var part in _creature.manikinLocations.PartList.GetAllParts())
            {
                foreach (var renderer in part.GetRenderers())
                {
                    foreach (var material in renderer.materials)
                    {
                        var materialName = material.name.ToLower();

                        if (
                            !materialName.Contains("body")
                            && !materialName.Contains("eye")
                            && !materialName.Contains("hair")
                            && !materialName.Contains("head")
                            && !materialName.Contains("male_hands")
                            && !materialName.Contains("mouth")
                        )

                            if (material.HasProperty("_BaseColor"))
                                material.SetColor("_BaseColor", new Color(25, 25, 25, 1));
                    }
                }
            }

            foreach (var holder in _creature.equipment.holders)
            {
                foreach (var item in holder.items)
                {
                    foreach (var renderer in item.renderers)
                    {
                        foreach (var material in renderer.materials)
                        {
                            material.SetColor("_BaseColor", new Color(25, 25, 25, 1));
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
                        foreach (var material in renderer.materials)
                        {
                            material.SetColor("_BaseColor", new Color(25, 25, 25, 1));
                        }
                    }
                }
            }

            _creature.OnKillEvent += (instance, time) =>
            {
                Player.local.creature.Heal(20, null);
                Destroy(this);
            };
            yield return null;
        }
    }
}
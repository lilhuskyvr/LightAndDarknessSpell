using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;
using Random = System.Random;

namespace LightAndDarknessSpell
{
    public class Banshee : MonoBehaviour
    {
        private Creature _creature;

        private void Start()
        {
            _creature = GetComponent<Creature>();
            StartCoroutine(BansheeTransformation());
        }

        private IEnumerator BansheeTransformation()
        {
            while (Time.time - _creature.spawnTime <= 1)
            {
                yield return new WaitForFixedUpdate();
            }

            var brain = _creature.brain.instance as BrainHuman;
            brain.canLeave = false;
            brain.allowDisarm = false;

            _creature.locomotion.speed = 2 * _creature.data.locomotionSpeed;
            _creature.animator.speed = 2;

            foreach (var part in _creature.manikinLocations.PartList.GetAllParts())
            {
                foreach (var renderer in part.GetRenderers())
                {
                    foreach (var material in renderer.materials)
                    {
                        material.SetColor("_BaseColor", Color.black);
                    }
                }
            }

            _creature.OnKillEvent += (instance, time) => { Destroy(this); };
            yield return null;
        }
    }
}
using System;
using System.Collections;
using ThunderRoad;
using UnityEngine;
using Random = System.Random;

namespace LightAndDarknessSpell
{
    public class Shadow : MonoBehaviour
    {
        private Creature _creature;

        public bool isSelfDestroy;
        public bool hasRagdoll;

        private void Awake()
        {
            _creature = GetComponent<Creature>();
        }

        public void Init(bool isSelfDestroyParam, bool hasRagdollParam)
        {
            isSelfDestroy = isSelfDestroyParam;
            hasRagdoll = hasRagdollParam;
            StartCoroutine(ShadowTransformation());
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

        private IEnumerator ShadowTransformation()
        {
            while (Time.time - _creature.spawnTime <= 1)
            {
                yield return new WaitForFixedUpdate();
            }

            _creature.ragdoll.enabled = hasRagdoll;
            _creature.locomotion.speed = 2 * _creature.data.locomotionSpeed;
            _creature.animator.speed = 2;

            foreach (var part in _creature.manikinLocations.PartList.GetAllParts())
            {
                foreach (var renderer in part.GetRenderers())
                {
                    foreach (var material in renderer.sharedMaterials)
                    {
                        if (material.HasProperty("_BaseColor"))
                        {
                            var materialName = material.name.ToLower();
                            
                            var color = materialName.Contains("eye") && !materialName.Contains("brow")? new Color(0,25, 0, 1): new
                                Color(0, 0, 0, 1);
                            material.SetColor("_BaseColor", color);
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